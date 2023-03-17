using BitApps.Core.Models;

namespace BitDesk.Models;

// APIリクエストの結果に含めて返すエラー情報保持クラス
public class ErrorInfo : ViewModelBase
{
    private string? _errorTitle;
    public string? ErrorTitle
    {
        get => _errorTitle;
        set
        {
            if (_errorTitle == value) {
                return;
            }
            _errorTitle = value;
            NotifyPropertyChanged(nameof(ErrorTitle));
        }
    }

    private int _errorCode;
    public int ErrorCode
    {
        get => _errorCode;
        set
        {
            if (_errorCode == value) {
                return;
            }

            _errorCode = value;
            NotifyPropertyChanged(nameof(ErrorCode));
        }
    }

    private string? _errorDescription;
    public string? ErrorDescription
    {
        get => _errorDescription;
        set
        {
            if (_errorDescription == value) {
                return;
            }

            _errorDescription = value;
            NotifyPropertyChanged(nameof(ErrorDescription));
        }
    }

    public ErrorInfo()
    {

    }
}
