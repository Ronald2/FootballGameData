using AutoMapper;
using Football.Application.DTOs;
using Football.Application.Interfaces;
using Football.Domain.Entities;
using Football.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Football.Application.Services
{
    /// <summary>
    /// Servicio de gestión de partidos de fútbol.
    /// </summary>
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IMapper _mapper;
        private readonly PaginationOptions _paginationOptions;

        public GameService(IGameRepository gameRepository, IMapper mapper, IOptions<PaginationOptions> paginationOptions)
        {
            _gameRepository = gameRepository;
            _mapper = mapper;
            _paginationOptions = paginationOptions.Value;
        }

        /// <summary>
        /// Obtiene un partido por su identificador.
        /// </summary>
        /// <param name="id">Identificador del partido.</param>
        /// <returns>DTO del partido o null si no existe.</returns>
        public async Task<GameDto?> GetByIdAsync(int id)
        {
            var game = await _gameRepository.GetByIdAsync(id);
            return game == null ? null : _mapper.Map<GameDto>(game);
        }

        /// <summary>
        /// Lista todos los partidos.
        /// </summary>
        /// <returns>Lista de partidos.</returns>
        public async Task<IEnumerable<GameDto>> ListAllAsync()
        {
            var games = await _gameRepository.ListAllAsync();
            return _mapper.Map<IEnumerable<GameDto>>(games);
        }

        /// <summary>
        /// Crea un nuevo partido.
        /// </summary>
        /// <param name="gameDto">DTO del partido a crear.</param>
        /// <returns>DTO del partido creado.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<GameDto> CreateAsync(GameDto gameDto)
        {
            if (gameDto == null) throw new ArgumentNullException(nameof(gameDto));
            var entity = _mapper.Map<Game>(gameDto);
            await _gameRepository.AddAsync(entity);
            var created = await _gameRepository.GetByIdAsync(entity.Id);
            return _mapper.Map<GameDto>(created!);
        }

        /// <summary>
        /// Actualiza un partido existente.
        /// </summary>
        /// <param name="gameDto">DTO del partido a actualizar.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task UpdateAsync(GameDto gameDto)
        {
            if (gameDto == null) throw new ArgumentNullException(nameof(gameDto));
            var existing = await _gameRepository.GetByIdAsync(gameDto.Id);
            if (existing == null) throw new KeyNotFoundException("Partido no encontrado.");
            _mapper.Map(gameDto, existing);
            await _gameRepository.UpdateAsync(existing);
        }

        /// <summary>
        /// Elimina un partido por su identificador.
        /// </summary>
        /// <param name="id">Identificador del partido.</param>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task DeleteAsync(int id)
        {
            var existing = await _gameRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Partido no encontrado.");
            await _gameRepository.DeleteAsync(existing);
        }

        /// <summary>
        /// Devuelve una página de partidos, normalizando los parámetros de paginación.
        /// </summary>
        /// <param name="page">Número de página (1-based). Si es menor a 1, se normaliza a 1.</param>
        /// <param name="pageSize">Tamaño de página. Si es menor a 1 o page es inválido, se normaliza a 20. Máximo permitido: 100.</param>
        /// <returns>Página de partidos.</returns>
        public async Task<PagedResultDto<GameDto>> ListAsync(int page = 1, int pageSize = 20)
        {
            int defaultPage = _paginationOptions.DefaultPage;
            int defaultPageSize = _paginationOptions.DefaultPageSize;
            int maxPageSize = _paginationOptions.MaxPageSize;
            if (page < defaultPage)
            {
                page = defaultPage;
                pageSize = defaultPageSize;
            }
            if (pageSize < 1) pageSize = defaultPageSize;
            if (pageSize > maxPageSize) pageSize = maxPageSize;

            var (entities, total) = await _gameRepository.GetPagedAsync(page, pageSize);
            var dtos = _mapper.Map<IEnumerable<GameDto>>(entities);

            return new PagedResultDto<GameDto>
            {
                Items = dtos,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}