using System;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace WebAppConfig
{
    public class AlbumsConfiguration
    {
        public string VisibleAlbumsFilter { get; set; }

        public Lazy<Func<Album, bool>> VisibleAlbumsFilterExpression
        {
            get
            {
                return new Lazy<Func<Album, bool>>(() =>
                {
                    var options = ScriptOptions.Default.AddReferences(typeof(Album).Assembly);
                    return CSharpScript.EvaluateAsync<Func<Album, bool>>(VisibleAlbumsFilter, options).GetAwaiter().GetResult();
                });
            }
        }
    }
}
