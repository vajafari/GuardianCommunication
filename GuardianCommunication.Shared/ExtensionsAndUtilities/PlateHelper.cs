using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;

namespace GuardianCommunication.Shared.ExtensionsAndUtilities
{
    public static class PlateHelper
    {
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public static bool TryParsNationalPlate(string plateString, out DtoNationalPlateSections plate)
        {
            try
            {
                var regexMath = Regex.Match(plateString, ServiceConstants.NationalPlateRegex);
                if (regexMath.Success)
                {
                    plate = new DtoNationalPlateSections
                    {
                        FirstNumber = regexMath.Groups[1].Value.Trim().ToInt32(),
                        SecondNumber = regexMath.Groups[3].Value.Trim().ToInt32(),
                        IranSerial = regexMath.Groups[4].Value.Trim().ToInt32(),
                        Letter = regexMath.Groups[2].Value.Trim(),
                    };
                    return true;
                }
            }
            catch
            {
                // ignored
            }
            plate = null;
            return false;

        }

        public static string ConvertChunksToKarnamaPlate(CarPlateTypeEnumeration plateType, params string[] plateChunks)
        {
            switch (plateType)
            {
                case CarPlateTypeEnumeration.National:
                    if (plateChunks.Length == 4)
                    {
                        return $"{plateChunks[0].Trim()} {plateChunks[1].Trim()} {plateChunks[2].Trim()} ایران{plateChunks[3].Trim()}";
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(plateType), plateType, null);
            }

            return null;
        }


    }
}
