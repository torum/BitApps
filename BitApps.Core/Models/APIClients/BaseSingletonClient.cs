using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BitApps.Core.Models.APIClients;

public abstract class BaseSingletonClient : IDisposable
{
    private static readonly Lock _locker = new();
    private static volatile HttpClient? _client;
    private static readonly Uri _publicAPIUri = new("https://public.bitbank.cc");

    protected static HttpClient Client
    {
        get
        {
            if (_client == null)
            {
                lock (_locker)
                {
                    _client ??= new HttpClient();
                    _client.BaseAddress = _publicAPIUri;

                    _client.DefaultRequestHeaders.Clear();
                    _client.DefaultRequestHeaders.ConnectionClose = false;
                    //_client.DefaultRequestHeaders.ConnectionClose = true;
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }
            }

            return _client;
        }
    }

    #region == Events ==

    public delegate void ClientDebugOutput(BaseSingletonClient sender, string data);

    public event ClientDebugOutput? DebugOutput;

    protected async void ToDebugWindow(string data)
    {
        // TODO:
        await Task.Run(() => { DebugOutput?.Invoke(this, data); });
    }

    #endregion

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _client?.Dispose();

            _client = null;
        }
    }
}