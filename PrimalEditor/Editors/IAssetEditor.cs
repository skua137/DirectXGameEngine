using PrimalEditor.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimalEditor.Editors
{
    public interface IAssetEditor
    {
        public Asset Asset { get; }

        public void SetAsset(Asset asset);
    }
}
