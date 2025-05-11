using AutoMapper;
using Football.Application.DTOs;
using Football.Application.Interfaces;
using Football.Domain.Entities;
using Football.Domain.Interfaces;
using Microsoft.Extensions.Options;
using System.Linq;

namespace Football.Application.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;
        private readonly PaginationOptions _paginationOptions;

        public TeamService(ITeamRepository teamRepository, IMapper mapper, IOptions<PaginationOptions> paginationOptions)
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
            _paginationOptions = paginationOptions.Value;
        }

        /// <summary>
        /// Obtiene un equipo por su identificador.
        /// </summary>
        /// <param name="id">Identificador del equipo.</param>
        /// <returns>El equipo encontrado o null si no existe.</returns>
        public async Task<TeamDto?> GetByIdAsync(int id)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            return team == null
                ? null
                : _mapper.Map<TeamDto>(team);
        }

        /// <summary>
        /// Crea un nuevo equipo.
        /// </summary>
        /// <param name="teamDto">Datos del equipo a crear.</param>
        /// <returns>El equipo creado.</returns>
        public async Task<TeamDto> CreateAsync(TeamDto teamDto)
        {
            var entity = _mapper.Map<Team>(teamDto);
            await _teamRepository.AddAsync(entity);

            var created = await _teamRepository.GetByIdAsync(entity.Id);
            return _mapper.Map<TeamDto>(created!);
        }

        /// <summary>
        /// Actualiza un equipo existente.
        /// </summary>
        /// <param name="teamDto">Datos del equipo a actualizar.</param>
        public async Task UpdateAsync(TeamDto teamDto)
        {
            var existing = await _teamRepository.GetByIdAsync(teamDto.Id);
            if (existing == null)
                throw new KeyNotFoundException("Equipo no encontrado.");

            _mapper.Map(teamDto, existing);

            await _teamRepository.UpdateAsync(existing);
        }

        /// <summary>
        /// Elimina un equipo por su identificador.
        /// </summary>
        /// <param name="id">Identificador del equipo a eliminar.</param>
        public async Task DeleteAsync(int id)
        {
            var existing = await _teamRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Equipo no encontrado.");

            await _teamRepository.DeleteAsync(existing);
        }

        /// <summary>
        /// Devuelve una página de equipos.
        /// </summary>
        /// <param name="page">Número de página.</param>
        /// <param name="pageSize">Tamaño de página.</param>
        /// <param name="nameFilter">Filtro por nombre del equipo.</param>
        /// <param name="coachFilter">Filtro por nombre del entrenador.</param>
        /// <returns>Una página de equipos.</returns>
        public async Task<PagedResultDto<TeamDto>> ListAsync(int page = 1, int pageSize = 20, string? nameFilter = null, string? coachFilter = null)
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

            var (entities, total) = await _teamRepository.GetPagedAsync(page, pageSize, nameFilter, coachFilter);
            
            var dtos = _mapper.Map<IEnumerable<TeamDto>>(entities);
            return new PagedResultDto<TeamDto>
            {
                Items = dtos,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}