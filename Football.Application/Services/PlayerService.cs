using AutoMapper;
using Football.Application.DTOs;
using Football.Application.Interfaces;
using Football.Domain.Entities;
using Football.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Football.Application.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IMapper _mapper;
        private readonly PaginationOptions _paginationOptions;

        public PlayerService(IPlayerRepository playerRepository, IMapper mapper, IOptions<PaginationOptions> paginationOptions)
        {
            _playerRepository = playerRepository;
            _mapper = mapper;
            _paginationOptions = paginationOptions.Value;
        }

        /// <summary>
        /// Obtiene un jugador por su ID.
        /// </summary>
        /// <param name="id">ID del jugador.</param>
        /// <returns>DTO del jugador.</returns>
        public async Task<PlayerDto?> GetByIdAsync(int id)
        {
            var player = await _playerRepository.GetByIdAsync(id);
            return player == null
                ? null
                : _mapper.Map<PlayerDto>(player);
        }

        /// <summary>
        /// Crea un nuevo jugador en un equipo.
        /// </summary>
        /// <param name="teamId">ID del equipo.</param>
        /// <param name="playerDto">DTO del jugador.</param>
        /// <returns>DTO del jugador creado.</returns>
        public async Task<PlayerDto> CreateAsync(int teamId, PlayerDto playerDto)
        {
            if (playerDto == null) throw new ArgumentNullException(nameof(playerDto));

            var entity = _mapper.Map<Player>(playerDto);
            entity.TeamId = teamId;
            await _playerRepository.AddAsync(entity);

            var created = await _playerRepository.GetByIdAsync(entity.Id);
            return _mapper.Map<PlayerDto>(created!);
        }

        /// <summary>
        /// Actualiza un jugador existente.
        /// </summary>
        /// <param name="playerDto">DTO del jugador.</param>
        public async Task UpdateAsync(PlayerDto playerDto)
        {
            if (playerDto == null) throw new ArgumentNullException(nameof(playerDto));

            var existing = await _playerRepository.GetByIdAsync(playerDto.Id);
            if (existing == null)
                throw new KeyNotFoundException("Jugador no encontrado.");

            _mapper.Map(playerDto, existing);
            await _playerRepository.UpdateAsync(existing);
        }

        /// <summary>
        /// Elimina un jugador por su ID.
        /// </summary>
        /// <param name="id">ID del jugador.</param>
        public async Task DeleteAsync(int id)
        {
            var existing = await _playerRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Jugador no encontrado.");

            await _playerRepository.DeleteAsync(existing);
        }

        /// <summary>
        /// Devuelve una página de jugadores para un equipo.
        /// </summary>
        /// <param name="teamId">ID del equipo.</param>
        /// <param name="page">Número de página.</param>
        /// <param name="pageSize">Tamaño de página.</param>
        public async Task<PagedResultDto<PlayerDto>> ListByTeamAsync(int teamId, int page = 1, int pageSize = 20)
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

            var (entities, total) = await _playerRepository
                .GetPagedByTeamAsync(teamId, page, pageSize);

            var dtos = _mapper.Map<IEnumerable<PlayerDto>>(entities);
            return new PagedResultDto<PlayerDto>
            {
                Items = dtos,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

    }
}