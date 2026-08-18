using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagerServer.Model.Obsolete.Obsolete54
{
    public static class CurrencyKeys
    {
        private static Dictionary<Guid, Currency> currencies = new Dictionary<Guid, Currency>();

        static CurrencyKeys()
        {
            var list = new Currency[]
            {
            new Currency(new Guid("721fcb16-5f80-4962-947d-5fdfc201ac29"), "USD", "US Dollar", 1m, "US$"),
            new Currency(new Guid("1a4b7523-54b9-4921-a441-67a9a0095d39"), "EUR", "Euro", 0.73m, "€"),
            new Currency(new Guid("d8db5ec6-da5b-4c61-9116-fadb78ed5af3"), "GBP", "British Pound", 0.59m, "£"),
            new Currency(new Guid("40b6c871-4b61-427d-99ba-70214ff25f34"), "INR", "Indian Rupee", 59.3m, "₹"),
            new Currency(new Guid("5846ab1b-fad4-44c6-92ea-e929cf5efe0f"), "AUD", "Australian Dollar", 1.08m, "A$"),
            new Currency(new Guid("16e8564e-4b5b-4ba8-9f9f-ea2c27cc913d"), "CAD", "Canadian Dollar", 1.09m, "C$"),
            new Currency(new Guid("c19ad3f3-ae5d-43fc-906a-5f94795e8717"), "AED", "Emirati Dirham", 3.67m, "AED"),
            new Currency(new Guid("99f3ebea-1caa-468d-ba4c-56c4dafab3ba"), "MYR", "Malaysian Ringgit", 3.21m, "RM"),
            new Currency(new Guid("fa380fc5-2ab4-4d12-b820-175bb7784b5a"), "CHF", "Swiss Franc", 0.89m),
            new Currency(new Guid("8670a196-9dbc-435e-a902-23e29ce0028c"), "CNY", "Chinese Yuan Renminbi", 6.25m),
            new Currency(new Guid("6bda4e28-6397-4bc9-8459-fb721bf2b867"), "THB", "Thai Baht", 32.5m, "฿"),
            new Currency(new Guid("c0ab77ea-630e-42f0-9c3e-524c172f4e0f"), "SAR", "Saudi Arabian Riyal", 3.75m),
            new Currency(new Guid("9d214808-beb3-4592-b51b-7a7909c19d34"), "NZD", "New Zealand Dollar", 1.78m, "NZ$"),
            new Currency(new Guid("ca4d17f3-c6b3-4ef5-af95-346239ddb469"), "JPY", "Japanese Yen", 103m, "JP¥", decimalPlaces: 0),
            new Currency(new Guid("24d538f3-0252-4a49-9532-c72c7b383c89"), "SGD", "Singapore Dollar", 1.25m, "S$"),
            new Currency(new Guid("5174b468-8633-4e16-98ae-9b2d334e8eaa"), "PHP", "Philippine Peso", 43.6m, "₱"),
            new Currency(new Guid("9a5d5811-bb8b-43bb-b31e-f678a75cfc83"), "TRY", "Turkish Lira", 2.08m, "₺"),
            new Currency(new Guid("ee87127d-1308-4a1b-9ec6-2e0fd7583010"), "HKD", "Hong Kong Dollar", 7.75m, "HK$"),
            new Currency(new Guid("68b8d7d3-6f66-4571-8dcb-5b2aabc73df8"), "IDR", "Indonesian Rupiah", 11825m, "Rp"),
            new Currency(new Guid("af901db6-8162-4e7b-9866-d2fefd1b3e54"), "ZAR", "South African Rand", 10.6m, "R"),
            new Currency(new Guid("c5aa32ef-faf5-4a74-b074-f87e6cef7d94"), "MXN", "Mexican Peso", 12.9m, "Mex$"),
            new Currency(new Guid("0aa6045b-c6ec-4311-bc2f-7f2c24b69b29"), "SEK", "Swedish Krona", 6.63m),
            new Currency(new Guid("3adb88b3-b7f9-4227-adc9-4716a48b7162"), "BRL", "Brazilian Real", 2.25m, "R$"),
            new Currency(new Guid("819bc4df-e9e2-4eb0-b52c-5440a612884b"), "HUF", "Hungarian Forint", 222m, "Ft"),
            new Currency(new Guid("553e15cd-e7d2-44c5-9efa-33c578c16737"), "PKR", "Pakistani Rupee", 98.4m),
            new Currency(new Guid("fa841125-647e-47be-aa3c-c37fc4570369"), "QAR", "Qatari Riyal", 3.64m),
            new Currency(new Guid("9d33d45c-ed72-4d0b-bf5b-985c373d8dda"), "OMR", "Omani Rial", 0.39m, decimalPlaces: 3),
            new Currency(new Guid("998db3dd-acb0-4c6c-b2ff-589b6ecac41a"), "KWD", "Kuwaiti Dinar", 0.28m, decimalPlaces: 3),
            new Currency(new Guid("2fb438e6-9ed4-4b72-af77-5f5d83e274e1"), "DKK", "Danish Krone", 5.47m),
            new Currency(new Guid("587d463a-e95c-42b5-b78b-9f9502f38286"), "NOK", "Norwegian Krone", 5.95m),
            new Currency(new Guid("e879905b-f829-42bc-ad7e-7f9ef2cd9f0c"), "RUB", "Russian Ruble", 34.5m, "₽") ,
            new Currency(new Guid("8c27f1ea-8403-49c9-8897-f4f65f215147"), "EGP", "Egyptian Pound", 7.15m),
            new Currency(new Guid("9d25f18f-f174-4137-8ef5-c09b2cac98ca"), "KRW", "South Korean Won", 1020m, "₩", decimalPlaces: 0),
            new Currency(new Guid("95db4af3-c293-473f-8aee-f2a2e4172e54"), "PLN", "Polish Zloty", 3m, "zł"),
            new Currency(new Guid("444855f6-d619-426f-8a2e-069998a45ac3"), "COP", "Colombian Peso", 1887m),
            new Currency(new Guid("52f42d91-fa63-4ee7-ad2f-7574ac1b672d"), "CZK", "Czech Koruna", 20.1m),
            new Currency(new Guid("b77615a5-938c-4a6d-9fe5-62c097d192be"), "ILS", "Israeli Shekel", 3.46m, "₪"),
            new Currency(new Guid("b29c5c89-233c-4c12-873e-8bfa63cc9a5d"), "IQD", "Iraqi Dinar", 1178m, decimalPlaces: 3),
            new Currency(new Guid("12f6fe6c-e6a7-43ac-83d0-c2786d185f41"), "NGN", "Nigerian Naira", 164m, "₦"),
            new Currency(new Guid("ed6bf1f6-a3ff-4d62-b558-58497d07577c"), "MAD", "Moroccan Dirham", 8.23m),
            new Currency(new Guid("f89e3d1b-1bcc-47da-9c5f-2c7a1d94a6de"), "ARS", "Argentine Peso", 8.09m),
            new Currency(new Guid("7b8cbc89-bcab-4da0-aaa4-a8f5cf65897f"), "LKR", "Sri Lankan Rupee", 130m, "Rs."),
            new Currency(new Guid("a381aab6-710c-4c11-90c7-d344cfd6ff22"), "TWD", "Taiwan New Dollar", 30m),
            new Currency(new Guid("41210cdb-00f3-463d-8fed-a9e2341e0ba7"), "BDT", "Bangladeshi Taka", 77.5m, "৳"),
            new Currency(new Guid("8165b346-9d1b-46c5-89a7-d130aa462e52"), "BHD", "Bahraini Dinar", 0.38m, decimalPlaces: 3),
            new Currency(new Guid("53196560-203e-428d-ab6c-7596492328c6"), "VND", "Vietnamese Dong", 21175m, "₫", decimalPlaces: 0),
            new Currency(new Guid("1d3510cd-c71c-49f3-9b4e-a641c4c10a23"), "CLP", "Chilean Peso", 550m, decimalPlaces: 0),
            new Currency(new Guid("5a6c6b49-ff7a-4ad4-9f43-4a4e19b64208"), "KES", "Kenyan Shilling", 87.5m),
            new Currency(new Guid("80953aa0-501d-453a-b525-fc056443534f"), "TND", "Tunisian Dinar", 1.64m, decimalPlaces: 3),
            new Currency(new Guid("d31f19fa-e8b4-4dd5-a2fe-f07ca7589d04"), "XOF", "CFA Franc", 482m, decimalPlaces: 0),
            new Currency(new Guid("e6cd31c4-4b65-47eb-b12b-2ca065595c18"), "JOD", "Jordanian Dinar", 0.71m, decimalPlaces: 3),
            new Currency(new Guid("48294b0f-1f28-4f3b-9c62-852905c91e6d"), "GHS", "Ghanaian Cedi", 3.05m, "GH₵"),
            new Currency(new Guid("7985cbfd-30c7-44dc-95c1-5d954ae5ef8f"), "HRK", "Croatian Kuna", 5.56m),
            new Currency(new Guid("020d4161-a004-4f60-8470-c35e988bf703"), "BGN", "Bulgarian Lev", 1.43m),
            new Currency(new Guid("eb9d3ea9-2def-4d97-b46e-840d2a0739ef"), "RON", "Romanian New Leu", 3.22m),
            new Currency(new Guid("28fec082-3d90-4410-813e-025425373b84"), "PEN", "Peruvian Nuevo Sol", 2.78m),
            new Currency(new Guid("b8a6fc7b-a650-49d5-98f7-35bc0be15f92"), "DZD", "Algerian Dinar", 79.2m),
            new Currency(new Guid("66493c67-c234-40ee-b338-fe3860b67a52"), "NPR", "Nepalese Rupee", 94.7m),
            new Currency(new Guid("b658d8e7-1331-4ed6-ba9f-32f332e87ec7"), "XAF", "Central African CFA Franc BEAC", 481m, decimalPlaces: 0),
            new Currency(new Guid("50d607a4-3e81-412a-b6f3-6373271ec4e4"), "ISK", "Icelandic Krona", 113m, decimalPlaces: 0),
            new Currency(new Guid("10fce1d9-bd78-4af9-aa23-834a495fe17d"), "UAH", "Ukrainian Hryvnia", 11.8m, "₴"),
            new Currency(new Guid("2fba8a72-866c-4dfa-b7f2-c29c31dbc15b"), "FJD", "Fijian Dollar", 1.84m),
            new Currency(new Guid("ca694526-5297-4e6f-9358-7d3dee10c113"), "DOP", "Dominican Peso", 43.3m, "RD$"),
            new Currency(new Guid("ff2f8aaf-e291-431d-868a-551c2d1d244a"), "XPF", "CFP Franc", 87.7m, decimalPlaces: 0),
            new Currency(new Guid("655326e0-a9fb-4bea-9ae7-ae89b1526d59"), "MUR", "Mauritian Rupee", 30.2m),
            new Currency(new Guid("b3e5dea1-1b50-4ccf-8b17-de237b17406a"), "AZN", "Azerbaijani New Manat", 0.78m),
            new Currency(new Guid("dcb861a6-ce18-406e-8d4e-3e2983412506"), "BAM", "Bosnian Convertible Marka", 1.43m),
            new Currency(new Guid("b5249fc4-a737-42ac-8fdc-cc6b990c6d28"), "IRR", "Iranian Rial", 25562m),
            new Currency(new Guid("11d13eb0-07a7-4252-9f59-4156f02ca1d2"), "RSD", "Serbian Dinar", 84.7m),
            new Currency(new Guid("aa487250-d1f6-445e-9ef1-1c940e4e6d45"), "LTL", "Lithuanian Litas", 2.53m),
            new Currency(new Guid("57f6fbda-142b-455c-a527-7af86fae2b2b"), "BND", "Bruneian Dollar", 1.25m),
            new Currency(new Guid("87be1570-1366-447e-a9e7-3b4bd22a2082"), "ETB", "Ethiopian Birr", 19.5m),
            new Currency(new Guid("422da665-a933-4a91-b391-b3c5f655aa48"), "CRC", "Costa Rican Colon", 553m, "₡"),
            new Currency(new Guid("a0967843-49b8-47c6-a8aa-5e5dc695595b"), "VEF", "Venezuelan Bolivar", 6.29m),
            new Currency(new Guid("f2f4ab0a-f3ac-4047-8f6e-a8c996f4cddb"), "AFN", "Afghan Afghani", 57.1m),
            new Currency(new Guid("3d88b64e-0c4c-4f93-a7e0-38047cddadbc"), "TZS", "Tanzanian Shilling", 1679m),
            new Currency(new Guid("571ae232-9411-4ef2-b5d3-2491a7ac131c"), "UGX", "Ugandan Shilling", 2560m),
            new Currency(new Guid("9b889fdd-2ece-4923-bc1b-90f502f7ac23"), "JMD", "Jamaican Dollar", 110m),
            new Currency(new Guid("a93acdb6-3d88-4c8d-a7d5-138ca4aeffa9"), "GEL", "Georgian Lari", 1.76m),
            new Currency(new Guid("23c914d9-860d-4885-9397-aa114d519f1a"), "BWP", "Botswana Pula", 8.82m),
            new Currency(new Guid("2e5e2362-81a8-4343-a5e6-455c0cf21321"), "ZMW", "Zambian Kwacha", 6.47m),
            new Currency(new Guid("4e85f94a-d560-4922-93f1-04f153b82abf"), "MMK", "Burmese Kyat", 968m),
            new Currency(new Guid("909b155f-7c7f-41d3-9495-0169a07922b6"), "GTQ", "Guatemalan Quetzal", 7.81m),
            new Currency(new Guid("905b3e1b-92f6-4cca-9f03-14aa14062dca"), "XCD", "East Caribbean Dollar", 2.70m),
            new Currency(new Guid("c441917f-21f8-41bf-b05a-94e39a69566a"), "LYD", "Libyan Dinar", 1.23m, decimalPlaces: 3),
            new Currency(new Guid("0dc4f4d1-6947-45e1-a331-25caad2962b8"), "MKD", "Macedonian Denar", 45.2m),
            new Currency(new Guid("67a3f215-28f6-4fa3-b811-9593a6594d1d"), "TTD", "Trinidadian Dollar", 1m),
            new Currency(new Guid("0780a992-2133-499a-a5bc-b93cd6644a62"), "MZN", "Mozambican Metical", 1m),
            new Currency(new Guid("cfefaefb-3e14-4aa9-a2e1-0d27532c5850"), "ALL", "Albanian Lek", 103m),
            new Currency(new Guid("ab7d60ea-b8b4-4c58-b506-a81a9411a925"), "BOB", "Bolivian Boliviano", 1m),
            new Currency(new Guid("cb738f11-e876-4219-b2dc-112a73bf19d7"), "KZT", "Kazakhstani Tenge", 1m, "₸"),
            new Currency(new Guid("5231eab5-91e8-48b2-aa15-eb6642baacda"), "BBD", "Barbadian or Bajan Dollar", 1m),
            new Currency(new Guid("e420a42e-56ed-4348-b505-89f74d767103"), "AOA", "Angolan Kwanza", 97.7m),
            new Currency(new Guid("7090ae87-f2f0-4f9f-ae84-eb98b2e3185a"), "KHR", "Cambodian Riel", 4048m, "៛"),
            new Currency(new Guid("4d448501-a9f9-4c96-86d1-b16525261e0a"), "AMD", "Armenian Dram", 416m),
            new Currency(new Guid("71ce3483-5457-48d0-8e32-fde9224bc385"), "UYU", "Uruguayan Peso", 23m),
            new Currency(new Guid("0740e0bf-8d0e-4285-a2b7-d35ffcac0209"), "MOP", "Macau Pataca", 7.97m),
            new Currency(new Guid("f15de936-00c8-4693-b7ad-3efa9ae49979"), "NAD", "Namibian Dollar", 10.6m, "N$"),
            new Currency(new Guid("85e6aee6-69c1-410f-a260-0b6042064263"), "LBP", "Lebanese Pound", 1511m),
            new Currency(new Guid("bcd8261d-3ef9-404e-8e92-18a38a909258"), "LAK", "Lao or Laotian Kip",80501m, "₭N"),
            new Currency(new Guid("6c626e8d-dcb3-4e1f-92e7-a2d12ad70fe2"), "BYR", "Belarusian Ruble", 10127m, decimalPlaces: 0),
            new Currency(new Guid("4d2f2df9-2d7d-44e5-a3c1-8506f593e16d"), "BYN", "Belarusian Ruble", 1.93m, decimalPlaces: 2),
            new Currency(new Guid("197d4901-72ac-49fc-810c-0530472a960d"), "MGA", "Malagasy Ariary", 2405m, decimalPlaces: 1),
            new Currency(new Guid("a28daa05-1a9c-4ca4-aae7-a6b40723a816"), "SYP", "Syrian Pound", 149m),
            new Currency(new Guid("2367ee3c-7851-44ed-b6d0-9d5043f5cff7"), "VUV", "Ni-Vanuatu Vatu", 95.8m, decimalPlaces: 0),
            new Currency(new Guid("199c8867-7250-4d99-90ce-d77699ce4e34"), "PGK", "Papua New Guinean Kina", 2.82m),
            new Currency(new Guid("ef28743c-3873-4e77-9aee-53e2bbae805d"), "MNT", "Mongolian Tughrik", 1814m, "₮"),
            new Currency(new Guid("4e7acabb-3080-48bb-8bb7-bb39263a271d"), "SDG", "Sudanese Pound", 5.69m),
            new Currency(new Guid("3b367b76-3451-4e41-b430-65265af11581"), "ANG", "Dutch Guilder", 1.79m),
            new Currency(new Guid("4c228bc8-1d7a-4014-abda-2dfc45667480"), "MWK", "Malawian Kwacha", 392m),
            new Currency(new Guid("d3d7d687-add4-437c-b83d-f249a64bc165"), "GMD", "Gambian Dalasi", 39.6m),
            new Currency(new Guid("949d328c-d076-4aa9-b45d-b66dcf0cc92d"), "CUP", "Cuban Peso", 0.99m),
            new Currency(new Guid("af1a786f-7638-417c-9396-11bee7f34b01"), "RWF", "Rwandan Franc", 679m, decimalPlaces: 0),
            new Currency(new Guid("dcfe4bc1-74fb-4e93-ae51-357e232c4144"), "MVR", "Maldivian Rufiyaa", 15.4m),
            new Currency(new Guid("3536c961-3607-4ffc-b5e9-1a5e73c314a6"), "BTN", "Bhutanese Ngultrum", 59.1m),
            new Currency(new Guid("b2050cb2-e34f-48a8-8b09-741902dfd755"), "SCR", "Seychellois Rupee", 12.1m),
            new Currency(new Guid("11720fbf-e8a7-44ea-a0f5-15e5047a88e2"), "HNL", "Honduran Lempira", 20.7m),
            new Currency(new Guid("c41e42a1-84fb-47d2-a1fe-c0f87fbc77c9"), "KPW", "North Korean Won", 900m),
            new Currency(new Guid("8fd765e4-f13c-4934-b773-eab7a1c3196a"), "PYG", "Paraguayan Guarani", 4416m, "₲", decimalPlaces: 0),
            new Currency(new Guid("d71380ae-65b0-46ee-a3ee-a6312ee085e8"), "DJF", "Djiboutian Franc", 178m, decimalPlaces: 0),
            new Currency(new Guid("770af059-4537-47c6-b5b6-d89139ebaef7"), "BTC", "Bitcoin", 0.0015m, decimalPlaces: 8),
            new Currency(new Guid("d897c502-5e8d-4301-be78-faae82907924"), "YER", "Yemeni Rial", 215m),
            new Currency(new Guid("611b768c-e655-4619-a306-19b53c8bf5a4"), "CDF", "Congolese Franc", 921m),
            new Currency(new Guid("1a463a42-9d36-4907-aef4-a2fbd39b4687"), "WST", "Samoan Tala", 2.31m),
            new Currency(new Guid("719ee9e5-6269-4423-943f-5964ba7d03ed"), "GYD", "Guyanese Dollar", 205m),
            new Currency(new Guid("75b688af-068a-45a9-b269-81057a208252"), "AWG", "Aruban or Dutch Guilder", 1.79m),
            new Currency(new Guid("5e512e1b-33c9-4563-aac0-dd0d409d6895"), "MDL", "Moldovan Leu", 13.8m),
            new Currency(new Guid("9dc88920-3296-46ca-adfb-f210140421d6"), "BZD", "Belizean Dollar", 1.99m),
            new Currency(new Guid("e06b373b-2efc-47b3-9a5c-8764b2dcddcf"), "HTG", "Haitian Gourde", 45.1m),
            new Currency(new Guid("6d2313f8-6a78-4f45-9f57-5d6f61a80715"), "KGS", "Kyrgyzstani Som", 52.5m),
            new Currency(new Guid("6a8e0db5-e369-4f50-9eb6-f491987b2a27"), "NIO", "Nicaraguan Cordoba", 25.8m),
            new Currency(new Guid("230e927d-d79d-4448-b082-8ae4cca976e1"), "CVE", "Cape Verdean Escudo", 80.4m, decimalPlaces: 0),
            new Currency(new Guid("cdeae396-7f16-4dca-a092-00fb49757ca2"), "KYD", "Caymanian Dollar", 0.82m),
            new Currency(new Guid("f7418c6a-5d63-4ac0-b263-127a059ee16a"), "GNF", "Guinean Franc", 7023m, decimalPlaces: 0),
            new Currency(new Guid("9ff67f25-b0df-4ee9-b372-94cec9dfed1b"), "BSD", "Bahamian Dollar", 1m),
            new Currency(new Guid("2aafdffc-5594-4a85-b09a-198eea6f8f0f"), "BIF", "Burundian Franc", 1544m, decimalPlaces: 0),
            new Currency(new Guid("3c3fb329-d105-4829-8819-7076c462e78b"), "SLL", "Sierra Leonean Leone", 4318m),
            new Currency(new Guid("909da993-c101-4b7d-97f4-1b51efc1d9b6"), "MRO", "Mauritanian Ouguiya", 293m, decimalPlaces: 1),
            new Currency(new Guid("90f39d4e-4fc5-4164-8e70-40f3ecdb4647"), "TOP", "Tongan Pa'anga", 1.86m),
            new Currency(new Guid("89d53144-e12b-405f-92b9-91a53995f762"), "BMD", "Bermudian Dollar", 1m),
            new Currency(new Guid("cd8de9da-a07f-4b97-a8cb-198a6a3cc3ce"), "SBD", "Solomon Islander Dollar", 7.28m),
            new Currency(new Guid("f72cb605-89fa-4964-974d-e38dbb3752be"), "UZS", "Uzbekistani Som", 2306m),
            new Currency(new Guid("c0e8412c-d8ae-4b81-8d1a-cca6dc4ac80b"), "SOS", "Somali Shilling", 941m),
            new Currency(new Guid("592852de-5ee8-4285-8e97-42410965b2ce"), "PAB", "Panamanian Balboa", 1m),
            new Currency(new Guid("5a228910-0339-4302-a0e5-92b0d1140e6f"), "SRD", "Surinamese Dollar", 3.31m),
            new Currency(new Guid("00669b87-daeb-4803-8ce3-c5b6ec06d9fc"), "SZL", "Swazi Lilangeni", 10.6m),
            new Currency(new Guid("e529770c-4d40-4901-90b6-753489f3d466"), "ERN", "Eritrean Nakfa", 14.9m),
            new Currency(new Guid("08206c4b-15db-4cf2-95a1-a19f5952bbcf"), "LRD", "Liberian Dollar", 84m),
            new Currency(new Guid("81168297-4092-4317-9901-1c8323b4dde1"), "TJS", "Tajikistani Somoni", 4.9m),
            new Currency(new Guid("493a8fa6-0825-4ffe-9df4-c953a06aca5b"), "TMT", "Turkmenistani Manat", 2.85m),
            new Currency(new Guid("1d8a4467-74e2-4f2d-b297-99fa6ba6b67a"), "GIP", "Gibraltar Pound", 0.59m),
            new Currency(new Guid("9e2c5fc4-7bf7-4206-a776-1611caebbc1c"), "LSL", "Basotho Loti", 10.6m),
            new Currency(new Guid("4fc8b2da-29dc-49da-9997-888248538c83"), "KMF", "Comoran Franc", 360m, decimalPlaces: 0),
            new Currency(new Guid("51be5dbf-1fef-46b8-b09d-6897fa8b3211"), "SVC", "Salvadoran Colon", 8.74m),
            new Currency(new Guid("2c5ef25f-5aec-442b-bc87-56454f526c34"), "STD", "Sao Tomean Dobra", 17965m),
            new Currency(new Guid("9ec01c7d-156c-48fa-be5e-dfce39c31c06"), "ETH", "Ethereum", 12m, decimalPlaces: 8),
            new Currency(new Guid("a8103b10-ddf4-407f-8616-444f5e3ab36a"), "ZWL", "ZWL Dollar", 4m, "ZWL$"),
            };
            foreach (var e in list)
            {
                currencies.Add(e.Key, e);
            }
        }

        public static Currency[] All
        {
            get
            {
                return currencies.Values.ToArray();
            }
        }

        public static Currency Get(Guid currency)
        {
            if (currencies.ContainsKey(currency)) return currencies[currency];
            return null;
        }

        public static int GetDecimalDigits(Guid? currency)
        {
            if (!currency.HasValue) return 2;
            if (!currencies.ContainsKey(currency.Value)) return 2;
            return Get(currency.Value).DecimalPlaces;
        }

        public sealed class Currency
        {
            public Currency(Guid key, string code, string name, decimal exchangeRate, string prefix = null, int decimalPlaces = 2)
            {
                Key = key;
                Code = code;
                Name = name;
                ExchangeRate = exchangeRate;
                Prefix = prefix;
                DecimalPlaces = decimalPlaces;
            }

            public Guid Key;
            public string Code;
            public string Prefix;
            public string Name;
            public decimal ExchangeRate;
            public int DecimalPlaces;

            public string GetDisplayName() { return Code + " - " + Name; }
        }
    }
}
