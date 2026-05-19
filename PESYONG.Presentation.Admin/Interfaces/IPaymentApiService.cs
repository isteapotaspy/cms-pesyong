using System;
using System.Collections.Generic;
using System.Text;
using CMS.Contracts.Admin.Payment;

namespace PESYONG.Presentation.Admin.Interfaces;

public interface IPaymentApiService
{
    Task<IReadOnlyList<PaymentDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PaymentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<PaymentDto> CreateAsync(CreatePaymentRequest request, CancellationToken cancellationToken = default);

    Task<PaymentDto> UpdateAsync(int id, UpdatePaymentRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
