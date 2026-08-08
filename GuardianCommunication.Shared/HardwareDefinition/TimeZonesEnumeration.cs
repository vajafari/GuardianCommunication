using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{
    [DataContract]
    public enum TimeZonesEnumeration
    {
        EniwetokKwajalein = -1200,
        MidwayIslandSamoa = -1100,
        Hawaii = -1000,
        Taiohae = -930,
        Alaska = -900,
        PacificTimeUsCanada = -800,
        MountainTimeUsCanada = -700,
        CentralTimeUsCanadaMexicoCity = -600,
        EasternTimeUsCanadaBogotaLima = -500,
        Caracas = -430,
        AtlanticTimeCanadaCaracasLaPaz = -400,
        Newfoundland = -330,
        BrazilBuenosAiresGeorgetown = -300,
        MidAtlantic = -200,
        AzoresCapeVerdeIslands = -100,
        WesternEuropeTimeLondonLisbonCasablanca = 0,
        BrusselsCopenhagenMadridParis = 100,
        KaliningradSouthAfrica = 200,
        BaghdadRiyadhMoscowStPetersburg = 300,
        Tehran = 330,
        AbuDhabiMuscatBakuTbilisi = 400,
        Kabul = 430,
        EkaterinburgIslamabadKarachiTashkent = 500,
        BombayCalcuttaMadrasNewDelhi = 530,
        KathmanduPokhara = 545,
        AlmatyDhakaColombo = 600,
        YangonMandalay = 630,
        BangkokHanoiJakarta = 700,
        BeijingPerthSingaporeHongKong = 800,
        Eucla = 845,
        TokyoSeoulOsakaSapporoYakutsk = 900,
        AdelaideDarwin = 930,
        EasternAustraliaGuamVladivostok = 1000,
        LordHoweIsland = 1030,
        MagadanSolomonIslandsNewCaledonia = 1100,
        NorfolkIsland = 1130,
        AucklandWellingtonFijiKamchatka = 1200,
        ChathamIslands = 1245,
        ApiaNukualofa = 1300,
        LineIslandsTokelau = 1400
    }

}
