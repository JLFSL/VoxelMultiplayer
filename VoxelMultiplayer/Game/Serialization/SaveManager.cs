using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using UnityEngine;

namespace VoxelMultiplayer.Game.Serialization
{
    class SaveManager
    {
        public static VoxelTycoon.Serialization.ExtendedSaveMetadata CreateMetadataForCurrentState(string filename)
        {
            Texture2D screenshot = VoxelTycoon.Helper.TakeScreenshot(120, 90, false);
            VoxelTycoon.Serialization.ExtendedSaveMetadata extendedSaveMetadata = new VoxelTycoon.Serialization.ExtendedSaveMetadata
            {
                Filename = Path.ChangeExtension(filename ?? SaveManager.GetNewSaveFilename(), "sav"),
                Screenshot = screenshot.EncodeToPNG(),
                FormattedMoney = VoxelTycoon.Game.UI.UIFormat.Money.Format(VoxelTycoon.Company.Current.Money),
                Playtime = VoxelTycoon.LazyManager<VoxelTycoon.TimeManager>.Current.UnscaledWorldTime,
                Date = DateTime.Now,
                BuildVersion = Application.version,
                Cheater = VoxelTycoon.Company.Current.Cheater,
                SeedString = VoxelTycoon.WorldSettings.Current.SeedString,
                Packs =
                VoxelTycoon.AssetManagement.EnabledPacksPerSaveHelper.GetEnabledPacks().Select<VoxelTycoon.AssetManagement.Pack,
                VoxelTycoon.Serialization.ExtendedSaveMetadata.SaveMetadataPack>((Func<VoxelTycoon.AssetManagement.Pack,
                VoxelTycoon.Serialization.ExtendedSaveMetadata.SaveMetadataPack>)(x => new VoxelTycoon.Serialization.ExtendedSaveMetadata.SaveMetadataPack()
                { Id = x.Id, Title = x.Title })).ToArray<VoxelTycoon.Serialization.ExtendedSaveMetadata.SaveMetadataPack>()
            };
            return extendedSaveMetadata;
        }

        private static string GetNewSaveFilename() => Path.GetRandomFileName().Replace(".", "");
    }
}
