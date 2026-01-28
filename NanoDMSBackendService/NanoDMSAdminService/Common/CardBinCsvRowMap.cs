using NanoDMSAdminService.DTO.CardBin;
using CsvHelper.Configuration;
namespace NanoDMSAdminService.Common
{
    public sealed class CardBinCsvRowMap : ClassMap<CardBinCsvRowDto>
    {
        public CardBinCsvRowMap()
        {
            Map(m => m.Bin).Name("BIN");
            Map(m => m.IssuingBank).Name("ISSUING BANK");
            Map(m => m.CardBrand).Name("CARD BRAND");
            Map(m => m.CardType).Name("CARD TYPE");
            Map(m => m.CardLevel).Name("CARD LEVEL");
            Map(m => m.Country).Name("COUNTRY");
            Map(m => m.LocalInternational).Name("LOCAL/INTERNATIONAL");
        }
    }
}
