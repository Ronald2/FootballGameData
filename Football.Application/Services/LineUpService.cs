using AutoMapper;
using Football.Application.DTOs;
using Football.Application.Interfaces;
using Football.Domain.Entities;
using Football.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Football.Application.Services
{
    public class LineUpService : ILineUpService
    {
        private readonly ILineUpRepository _lineUpRepository;
        private readonly IMapper _mapper;
        private readonly PaginationOptions _paginationOptions;

        public LineUpService(ILineUpRepository lineUpRepository, IMapper mapper, IOptions<PaginationOptions> paginationOptions)
        {
            _lineUpRepository = lineUpRepository;
            _mapper = mapper;
            _paginationOptions = paginationOptions.Value;
        }

        /// <summary>
        /// Obtiene una alineación por su ID.
        /// </summary>
        /// <param name="id">ID de la alineación.</param>
        /// <returns>Alineación encontrada o null si no existe.</returns>
        public async Task<LineUpDto?> GetByIdAsync(int id)
        {
            var lineUp = await _lineUpRepository.GetByIdAsync(id);
            return lineUp == null ? null : _mapper.Map<LineUpDto>(lineUp);
        }

        /// <summary>
        /// Devuelve una página de alineaciones para un partido.
        /// </summary>
        /// <param name="gameId">ID del partido.</param>
        /// <param name="page">Número de página.</param>
        /// <param name="pageSize">Tamaño de página.</param>
        /// <returns>Resultado paginado de alineaciones.</returns>
        public async Task<PagedResultDto<LineUpDto>> ListByGameAsync(int gameId, int page = 1, int pageSize = 20)
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

            var (entities, total) = await _lineUpRepository.GetPagedByGameAsync(gameId, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<LineUpDto>>(entities);

            return new PagedResultDto<LineUpDto>
            {
                Items = dtos,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Crea una nueva alineación para un partido.
        /// </summary>
        /// <param name="gameId">ID del partido.</param>
        /// <param name="lineUpDto">Datos de la alineación.</param>
        /// <returns>Alineación creada.</returns>
        public async Task<LineUpDto> CreateAsync(int gameId, LineUpDto lineUpDto)
        {
            if (lineUpDto == null) throw new ArgumentNullException(nameof(lineUpDto));

            var entity = _mapper.Map<LineUp>(lineUpDto);
            entity.GameId = gameId;

            if (entity.PlayerId == 0)
                throw new ArgumentException("PlayerId is required.");
            if (entity.TeamId == 0)
                throw new ArgumentException("TeamId is required.");
            if (entity.GameId == 0)
                throw new ArgumentException("GameId is required.");

            await _lineUpRepository.AddAsync(entity);

            var created = await _lineUpRepository.GetByIdAsync(entity.Id);
            return _mapper.Map<LineUpDto>(created!);
        }

        /// <summary>
        /// Actualiza una alineación existente.
        /// </summary>
        /// <param name="lineUpDto">Datos de la alineación.</param>
        public async Task UpdateAsync(LineUpDto lineUpDto)
        {
            if (lineUpDto == null) throw new ArgumentNullException(nameof(lineUpDto));

            var existing = await _lineUpRepository.GetByIdAsync(lineUpDto.Id);
            if (existing == null)
                throw new KeyNotFoundException($"LineUp no encontrado (Id: {lineUpDto.Id}).");

            _mapper.Map(lineUpDto, existing);
            await _lineUpRepository.UpdateAsync(existing);
        }

        /// <summary>
        /// Elimina una alineación por su ID.
        /// </summary>
        /// <param name="id">ID de la alineación.</param>
        public async Task DeleteAsync(int id)
        {
            var existing = await _lineUpRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"LineUp no encontrado (Id: {id}).");

            await _lineUpRepository.DeleteAsync(existing);
        }
    }
}