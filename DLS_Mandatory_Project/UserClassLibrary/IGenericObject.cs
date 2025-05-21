using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserClassLibrary
{
    public interface IGenericObject
    {
        int Id { get; set; }
        void Validate();
    }
}
