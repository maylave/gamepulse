    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using WebApplication1.Models;
    using WebApplication1.Services;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Extensions.Options;
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.EntityFrameworkCore;

    namespace WebApplication1.Controllers.Api
    {
        [ApiController]
        [Route("api/[controller]")]
        public class GamesController : ControllerBase
        {
            private readonly ApplicationContext _context;
            private readonly UserManager<User> _userManager;

            public GamesController(ApplicationContext context, UserManager<User> userManager)
            {
                _context = context;
                _userManager = userManager;
            }

            private class GameDto
            {
                public int Id { get; set; }
                public string Title { get; set; } = string.Empty;
                public string Description { get; set; } = string.Empty;
                public decimal Price { get; set; }
                public decimal? OldPrice { get; set; }
                public string? Tag { get; set; }
                public string Developer { get; set; } = string.Empty;
                public string? Publisher { get; set; }
                public int AgeRating { get; set; }
                public DateTime ReleaseDate { get; set; }
                public string? ImageUrl { get; set; }
                public string? Category { get; set; }
                public bool IsPreorder { get; set; }
                public string? ExternalUrl { get; set; }

                public List<string> Genres { get; set; } = new();
                public List<ReviewDto> Reviews { get; set; } = new();
                public List<MediaDto> Media { get; set; } = new();
            }

            public class ReviewDto
            {
                public int Id { get; set; }
                public string UserName { get; set; } = "Аноним";
                public int Rating { get; set; }
                public string Text { get; set; } = string.Empty;
                public DateTime Date { get; set; }
            }

            private class MediaDto
            {
                public int Id { get; set; }
                public string Url { get; set; } = string.Empty;
                public string Type { get; set; } = "image";
            }

            public class CreateMediaDto
            {
                public string Url { get; set; } = string.Empty;
                public string Type { get; set; } = "image";
            }

            public class CreateGameDto
            {
                public string Title { get; set; } = string.Empty;
                public string Description { get; set; } = string.Empty;
                public decimal Price { get; set; }
                public decimal? OldPrice { get; set; }
                public string? Tag { get; set; }
                public string Developer { get; set; } = string.Empty;
                public string? Publisher { get; set; }
                public int AgeRating { get; set; }
                public DateTime ReleaseDate { get; set; }
                public string ImageUrl { get; set; } = string.Empty;
                public string Category { get; set; } = "other";
                public bool IsPreorder { get; set; }
                public string? ExternalUrl { get; set; }
                public List<int> GenreIds { get; set; } = new();
                public List<CreateMediaDto> Media { get; set; } = new();
            }

            public class BulkPurchaseDto
            {
                public List<BulkPurchaseItem> Items { get; set; } = new();
            }

            public class BulkPurchaseItem
            {
                public int GameId { get; set; }
                public int Quantity { get; set; }
                public decimal Price { get; set; }
            }

            [HttpGet]
            public async Task<IActionResult> GetGames(
                [FromQuery] string? search = null,
                [FromQuery] string? category = null,
                [FromQuery] int? minAge = null,
                [FromQuery] int? maxAge = null,
                [FromQuery] decimal? minPrice = null,
                [FromQuery] decimal? maxPrice = null,
                [FromQuery] bool? onSale = null,
                [FromQuery] List<int> genreIds = null,
                [FromQuery] string? sortBy = "id",
                [FromQuery] bool ascending = true,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 24)
            {
                page = Math.Max(page, 1);
                pageSize = Math.Clamp(pageSize, 1, 100);

                var query = _context.Games.AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var normalized = search.Trim().ToLower();
                    query = query.Where(g =>
                        (!string.IsNullOrEmpty(g.Title) && g.Title.ToLower().Contains(normalized)) ||
                        (!string.IsNullOrEmpty(g.Description) && g.Description.ToLower().Contains(normalized)) ||
                        (!string.IsNullOrEmpty(g.Developer) && g.Developer.ToLower().Contains(normalized)) ||
                        (!string.IsNullOrEmpty(g.Publisher) && g.Publisher.ToLower().Contains(normalized))
                    );
                }

                if (!string.IsNullOrWhiteSpace(category) && !string.Equals(category, "all", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(g =>
                        !string.IsNullOrEmpty(g.Category) &&
                        g.Category.ToLower() == category.Trim().ToLower());
                }

                if (minAge.HasValue) query = query.Where(g => g.AgeRating >= minAge.Value);
                if (maxAge.HasValue) query = query.Where(g => g.AgeRating <= maxAge.Value);

                if (minPrice.HasValue) query = query.Where(g => g.Price >= minPrice.Value);
                if (maxPrice.HasValue) query = query.Where(g => g.Price <= maxPrice.Value);

                if (onSale == true)
                    query = query.Where(g => g.OldPrice.HasValue);

                if (genreIds != null && genreIds.Any())
                {
                    query = query.Where(g => g.GameGenres.Any(gg => genreIds.Contains(gg.GenreId)));
                }

                query = sortBy?.ToLower() switch
                {
                    "title" => ascending
                        ? query.OrderBy(g => g.Title)
                        : query.OrderByDescending(g => g.Title),
                    "price" => ascending
                        ? query.OrderBy(g => g.Price)
                        : query.OrderByDescending(g => g.Price),
                    "releaseDate" => ascending
                        ? query.OrderBy(g => g.ReleaseDate)
                        : query.OrderByDescending(g => g.ReleaseDate),
                    _ => ascending
                        ? query.OrderBy(g => g.Id)
                        : query.OrderByDescending(g => g.Id)
                };

                var total = await query.CountAsync();

                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(g => new GameDto
                    {
                        Id = g.Id,
                        Title = g.Title ?? string.Empty,
                        Description = g.Description ?? string.Empty,
                        Price = g.Price,
                        OldPrice = g.OldPrice,
                        Tag = g.Tag,
                        Developer = g.Developer ?? string.Empty,
                        Publisher = g.Publisher,
                        AgeRating = g.AgeRating,
                        ReleaseDate = g.ReleaseDate,
                        ImageUrl = g.ImageUrl,
                        Category = g.Category,
                        IsPreorder = g.IsPreorder,
                        ExternalUrl = g.ExternalUrl,
                        Genres = g.GameGenres.Select(gg => gg.Genre.Name).ToList()
                    })
                    .ToListAsync();

                return Ok(new
                {
                    items,
                    total,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling((double)total / pageSize),
                    hasMore = (page * pageSize) < total
                });
            }
            [HttpGet("genres")]
    public async Task<IActionResult> GetGenres()
    {
        var genres = await _context.Genres
            .Select(g => new { g.Id, g.Name })
            .ToListAsync();
        return Ok(genres);
    }
            [HttpGet("purchased")]
            [Authorize]
            public async Task<IActionResult> GetPurchasedGames()
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                var purchasedGameIds = await _context.Purchases
                    .Where(p => p.UserId == user.Id)
                    .Select(p => p.GameId)
                    .Distinct()
                    .ToListAsync();

                if (!purchasedGameIds.Any())
                {
                    return Ok(Array.Empty<GameDto>());
                }

                var games = await _context.Games
                    .Where(g => purchasedGameIds.Contains(g.Id))
                    .Include(g => g.GameGenres).ThenInclude(gg => gg.Genre)
                    .Include(g => g.Media)
                    .Select(g => new GameDto
                    {
                        Id = g.Id,
                        Title = g.Title ?? string.Empty,
                        Description = g.Description ?? string.Empty,
                        Price = g.Price,
                        OldPrice = g.OldPrice,
                        Tag = g.Tag,
                        Developer = g.Developer ?? string.Empty,
                        Publisher = g.Publisher,
                        AgeRating = g.AgeRating,
                        ReleaseDate = g.ReleaseDate,
                        ImageUrl = g.ImageUrl,
                        Category = g.Category,
                        IsPreorder = g.IsPreorder,
                        ExternalUrl = g.ExternalUrl,
                        Genres = g.GameGenres.Select(gg => gg.Genre.Name).ToList(),
                        Media = g.Media.Select(m => new MediaDto
                        {
                            Id = m.Id,
                            Url = m.Url,
                            Type = m.Type
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(games);
            }

            [HttpPost("purchase")]
            [Authorize]
            public async Task<IActionResult> PurchaseGames([FromBody] BulkPurchaseDto request)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                var userId = user.Id;
                var totalAmount = 0m;
                var purchasedItems = new List<Purchase>();

                foreach (var item in request.Items)
                {
                    var game = await _context.Games
                        .Where(g => g.Id == item.GameId && !g.IsPreorder)
                        .FirstOrDefaultAsync();

                    if (game == null)
                        return NotFound($"Игра с ID {item.GameId} не найдена или недоступна");

                

                    if (Math.Abs(game.Price - item.Price) > 0.01m)
                        return BadRequest($"Цена игры '{game.Title}' изменилась. Пожалуйста, обновите корзину.");

                    var purchase = new Purchase
                    {
                        UserId = userId,
                        GameId = item.GameId,
                        Price = game.Price,
                        Quantity = item.Quantity,
                        PurchasedAt = DateTime.UtcNow
                    };

                    purchasedItems.Add(purchase);
                    totalAmount += game.Price * item.Quantity;
                }

                _context.Purchases.AddRange(purchasedItems);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Заказ успешно оформлен",
                    totalAmount = totalAmount,
                    items = purchasedItems.Select(p => new
                    {
                        gameId = p.GameId,
                        quantity = p.Quantity,
                        price = p.Price
                    }).ToList()
                });
            }

            [HttpGet("{id:int}")]
            public async Task<IActionResult> GetGame(int id)
            {
                var game = await _context.Games
                    .Where(g => g.Id == id)
                    .Include(g => g.GameGenres).ThenInclude(gg => gg.Genre)
                    .Include(g => g.Reviews).ThenInclude(r => r.User)
                    .Include(g => g.Media)
                    .Select(g => new GameDto
                    {
                        Id = g.Id,
                        Title = g.Title ?? string.Empty,
                        Description = g.Description ?? string.Empty,
                        Price = g.Price,
                        OldPrice = g.OldPrice,
                        Tag = g.Tag,
                        Developer = g.Developer ?? string.Empty,
                        Publisher = g.Publisher,
                        AgeRating = g.AgeRating,
                        ReleaseDate = g.ReleaseDate,
                        ImageUrl = g.ImageUrl,
                        Category = g.Category,
                        IsPreorder = g.IsPreorder,
                        ExternalUrl = g.ExternalUrl,
                        Genres = g.GameGenres.Select(gg => gg.Genre.Name).ToList(),
                        Reviews = g.Reviews
                            .OrderByDescending(r => r.CreatedAt)
                            .Select(r => new ReviewDto
                            {
                                Id = r.Id,
                                UserName = "Аноним",
                                Rating = r.Rating,
                                Text = r.Text,
                                Date = r.CreatedAt
                            }).ToList(),
                        Media = g.Media
                            .OrderBy(m => m.Id)
                            .Select(m => new MediaDto
                            {
                                Id = m.Id,
                                Url = m.Url,
                                Type = m.Type
                            })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                if (game == null)
                    return NotFound();

                return Ok(game);
            }

            [HttpPost]
            public async Task<IActionResult> AddGame([FromBody] CreateGameDto dto)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (string.IsNullOrWhiteSpace(dto.Title))
                    return BadRequest("Название игры не может быть пустым.");

                if (dto.GenreIds == null || !dto.GenreIds.Any())
                    return BadRequest("Необходимо указать хотя бы один жанр.");

                var existingGenres = await _context.Genres
                    .Where(g => dto.GenreIds.Contains(g.Id))
                    .ToListAsync();

                if (existingGenres.Count != dto.GenreIds.Distinct().Count())
                    return BadRequest("Один или несколько жанров не найдены.");

                var game = new Game
                {
                    Title = dto.Title.Trim(),
                    Description = dto.Description?.Trim() ?? string.Empty,
                    Price = dto.Price,
                    OldPrice = dto.OldPrice,
                    Tag = dto.Tag?.Trim(),
                    Developer = dto.Developer?.Trim() ?? string.Empty,
                    Publisher = dto.Publisher?.Trim() ?? string.Empty,
                    AgeRating = dto.AgeRating,
                    ReleaseDate = dto.ReleaseDate,
                    ImageUrl = dto.ImageUrl?.Trim(),
                    Category = dto.Category?.Trim() ?? "other",
                    IsPreorder = dto.IsPreorder,
                    ExternalUrl = dto.ExternalUrl?.Trim()
                };

                foreach (var genre in existingGenres)
                {
                    game.GameGenres.Add(new GameGenre { GenreId = genre.Id });
                }

                if (dto.Media != null && dto.Media.Any())
                {
                    foreach (var mediaDto in dto.Media)
                    {
                        if (string.IsNullOrWhiteSpace(mediaDto.Url)) continue;

                        game.Media.Add(new GameMedia
                        {
                            Url = mediaDto.Url.Trim(),
                            Type = mediaDto.Type?.ToLower() == "video" ? "video" : "image"
                        });
                    }
                }

                _context.Games.Add(game);
                await _context.SaveChangesAsync();

                var gameDto = new GameDto
                {
                    Id = game.Id,
                    Title = game.Title,
                    Description = game.Description,
                    Price = game.Price,
                    OldPrice = game.OldPrice,
                    Tag = game.Tag,
                    Developer = game.Developer,
                    Publisher = game.Publisher,
                    AgeRating = game.AgeRating,
                    ReleaseDate = game.ReleaseDate,
                    ImageUrl = game.ImageUrl,
                    Category = game.Category,
                    IsPreorder = game.IsPreorder,
                    ExternalUrl = game.ExternalUrl,
                    Genres = existingGenres.Select(g => g.Name).ToList(),
                    Media = game.Media.Select(m => new MediaDto
                    {
                        Id = m.Id,
                        Url = m.Url,
                        Type = m.Type
                    }).ToList()
                };

                return CreatedAtAction(nameof(GetGame), new { id = game.Id }, gameDto);
            }
        }
    }