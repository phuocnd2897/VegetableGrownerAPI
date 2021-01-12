using System;
using System.Collections.Generic;
using System.Text;

namespace VG.Context
{
    public interface IUnitOfWork
    {
        VGContext dbContext { get; }
    }
}
