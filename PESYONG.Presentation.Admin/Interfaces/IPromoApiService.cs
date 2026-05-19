using System;
using System.Collections.Generic;
using System.Text;
using CMS.Contracts.Admin.Promos;

namespace PESYONG.Presentation.Admin.Interfaces;

public interface IPromoApiService
{
    Task<IReadOnlyList<PromoDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PromoDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<PromoDto> CreateAsync(CreatePromoRequest request, CancellationToken cancellationToken = default);

    Task<PromoDto> UpdateAsync(int id, UpdatePromoRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
