using SalesTaxes.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesTaxes.Contracts.Services;

public interface ISalesTaxService
{
    decimal CalculateSalesTax(ProductDTO product, bool isTaxFreeState);
}
