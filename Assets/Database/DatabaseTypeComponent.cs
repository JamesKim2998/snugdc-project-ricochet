using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Database
{
    public interface IDatabaseTypeComponent<out T>
    {
        T Type();
    }

}
