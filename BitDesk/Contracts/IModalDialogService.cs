using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitDesk.ViewModels;

namespace BitDesk.Contracts;

public interface IModalDialogService
{
    void ShowOrderDialog(PairViewModel vm);
}
