using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace WebAppConfig
{
    public class AlbumsConfiguration
    {
        private static SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private static Dictionary<string, Func<Album, bool>> _visibleAlbumFilters = new Dictionary<string, Func<Album, bool>>();

        public string VisibleAlbumsFilter { get; set; }

        public async Task<Func<Album, bool>> GetVisibleAlbumsFilter()
        {
            if (_visibleAlbumFilters.ContainsKey(VisibleAlbumsFilter))
            {
                return _visibleAlbumFilters[VisibleAlbumsFilter];
            }

            await _lock.WaitAsync();
            try
            {
                var options = ScriptOptions.Default.AddReferences(typeof(Album).Assembly);
                var filter = await CSharpScript.EvaluateAsync<Func<Album, bool>>(VisibleAlbumsFilter, options);
                _visibleAlbumFilters[VisibleAlbumsFilter] = filter;
                return filter;
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
