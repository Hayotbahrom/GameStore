// <copyright file="PlatformService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AutoMapper;
    using GameStore.Application.DTOs.Platforms;
    using GameStore.Application.Interfaces;
    using GameStore.Domain.Entities;
    using GameStore.Infrastructure.IRepositories;
    using Microsoft.EntityFrameworkCore;
    public class PlatformService : IPlatformService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public PlatformService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        /// <inheritdoc />
        public async Task<PlatformForResultDto> CreatePlatformAsync(PlatformForCreationDto dto)
        {
            var platform = new Platform
            {
                Id = Guid.NewGuid(),
                Type = dto.Type
            };

            await this.unitOfWork.Platforms.AddAsync(platform);
            await this.unitOfWork.SaveChangesAsync();
            return this.mapper.Map<PlatformForResultDto>(platform);
        }

        /// <inheritdoc />
        public async Task<bool> DeletePlatformAsync(Guid id)
        {
            var platform = await this.unitOfWork.Platforms.FindByIdAsync(id);
            if (platform == null)
            {
                return false;
            }

            this.unitOfWork.Platforms.Delete(platform);
            await this.unitOfWork.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PlatformForResultDto>> GetAllPlatformsAsync()
        {
            var platforms = await this.unitOfWork.Platforms.GetAll().ToListAsync();
            return this.mapper.Map<IEnumerable<PlatformForResultDto>>(platforms);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PlatformForResultDto>> GetByGameKeyAsync(string gameKey)
        {
            var platforms = await this.unitOfWork.Platforms.GetByGameKeyAsync(gameKey);
            return this.mapper.Map<IEnumerable<PlatformForResultDto>>(platforms);
        }

        /// <inheritdoc />
        public async Task<PlatformForResultDto?> GetPlatformByIdAsync(Guid id)
        {
            var platform = await this.unitOfWork.Platforms.FindByIdAsync(id);
            if (platform == null)
            {
                return null;
            }
            return this.mapper.Map<PlatformForResultDto>(platform);
        }

        /// <inheritdoc />
        public async Task<bool> UpdatePlatformAsync(PlatformForUpdateDto platform)
        {
            var existingPlatform = await this.unitOfWork.Platforms.FindByIdAsync(platform.Id);
            if (existingPlatform == null)
            {
                return false;
            }

            existingPlatform.Type = platform.Type;
            await this.unitOfWork.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Guid>> GetPlatformIdsByGameNamesAsync(IEnumerable<string> gameNames)
        {
            var platforms = await this.unitOfWork.Platforms.GetAll().ToListAsync();
            return platforms
                .Where(p => gameNames.Contains(p.Type))
                .Select(p => p.Id);
        }
    }
}
