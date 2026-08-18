// Slovak standard chart of accounts ("Rámcová účtová osnova pre podnikateľov",
// Príloha č. 1 k opatreniu MF SR č. 23054/2002-92). Synthetic accounts only.
//
// Type: "bs" = BalanceSheetAccount (classes 0–4), "pl" = ProfitAndLossStatementAccount (classes 5–6).
// Class 7 (závierkové a podsúvahové účty) is not included — those are not user-entered.
// Match is by Code; the user can re-classify groups later inside Manager.

const COA_TEMPLATE = [
  // Trieda 0 — Dlhodobý majetok
  { code: "011", name: "Aktivované náklady na vývoj", type: "bs" },
  { code: "012", name: "Aktivované náklady na vývoj (obsolete)", type: "bs" },
  { code: "013", name: "Softvér", type: "bs" },
  { code: "014", name: "Oceniteľné práva", type: "bs" },
  { code: "015", name: "Goodwill", type: "bs" },
  { code: "019", name: "Ostatný dlhodobý nehmotný majetok", type: "bs" },
  { code: "021", name: "Stavby", type: "bs" },
  { code: "022", name: "Samostatné hnuteľné veci a súbory hnuteľných vecí", type: "bs" },
  { code: "025", name: "Pestovateľské celky trvalých porastov", type: "bs" },
  { code: "026", name: "Základné stádo a ťažné zvieratá", type: "bs" },
  { code: "029", name: "Ostatný dlhodobý hmotný majetok", type: "bs" },
  { code: "031", name: "Pozemky", type: "bs" },
  { code: "032", name: "Umelecké diela a zbierky", type: "bs" },
  { code: "041", name: "Obstaranie dlhodobého nehmotného majetku", type: "bs" },
  { code: "042", name: "Obstaranie dlhodobého hmotného majetku", type: "bs" },
  { code: "043", name: "Obstaranie dlhodobého finančného majetku", type: "bs" },
  { code: "051", name: "Poskytnuté preddavky na dlhodobý nehmotný majetok", type: "bs" },
  { code: "052", name: "Poskytnuté preddavky na dlhodobý hmotný majetok", type: "bs" },
  { code: "053", name: "Poskytnuté preddavky na dlhodobý finančný majetok", type: "bs" },
  { code: "061", name: "Podielové cenné papiere a podiely v dcérskej účtovnej jednotke", type: "bs" },
  { code: "062", name: "Podielové cenné papiere a podiely v účt. jednotke s podst. vplyvom", type: "bs" },
  { code: "063", name: "Realizovateľné cenné papiere a podiely", type: "bs" },
  { code: "065", name: "Dlhové cenné papiere držané do splatnosti", type: "bs" },
  { code: "066", name: "Pôžičky účt. jednotkám v konsolidovanom celku", type: "bs" },
  { code: "067", name: "Ostatné pôžičky", type: "bs" },
  { code: "069", name: "Ostatný dlhodobý finančný majetok", type: "bs" },
  { code: "072", name: "Oprávky k aktivovaným nákladom na vývoj", type: "bs" },
  { code: "073", name: "Oprávky k softvéru", type: "bs" },
  { code: "074", name: "Oprávky k oceniteľným právam", type: "bs" },
  { code: "075", name: "Oprávky ku goodwillu", type: "bs" },
  { code: "079", name: "Oprávky k ostatnému dlhodobému nehmotnému majetku", type: "bs" },
  { code: "081", name: "Oprávky k stavbám", type: "bs" },
  { code: "082", name: "Oprávky k samostatným hnuteľným veciam a súborom hnut. vecí", type: "bs" },
  { code: "085", name: "Oprávky k pestovateľským celkom trvalých porastov", type: "bs" },
  { code: "086", name: "Oprávky k základnému stádu a ťažným zvieratám", type: "bs" },
  { code: "089", name: "Oprávky k ostatnému dlhodobému hmotnému majetku", type: "bs" },
  { code: "091", name: "Opravná položka k dlhodobému nehmotnému majetku", type: "bs" },
  { code: "092", name: "Opravná položka k dlhodobému hmotnému majetku", type: "bs" },
  { code: "093", name: "Opravná položka k nedokončenému dlhodobému nehmot. majetku", type: "bs" },
  { code: "094", name: "Opravná položka k nedokončenému dlhodobému hmot. majetku", type: "bs" },
  { code: "095", name: "Opravná položka k poskytnutým preddavkom na DLM", type: "bs" },
  { code: "096", name: "Opravná položka k dlhodobému finančnému majetku", type: "bs" },
  { code: "097", name: "Opravná položka k nadobudnutému majetku", type: "bs" },
  { code: "098", name: "Oprávky k opravnej položke k nadobudnutému majetku", type: "bs" },

  // Trieda 1 — Zásoby
  { code: "111", name: "Obstaranie materiálu", type: "bs" },
  { code: "112", name: "Materiál na sklade", type: "bs" },
  { code: "119", name: "Materiál na ceste", type: "bs" },
  { code: "121", name: "Nedokončená výroba", type: "bs" },
  { code: "122", name: "Polotovary vlastnej výroby", type: "bs" },
  { code: "123", name: "Výrobky", type: "bs" },
  { code: "124", name: "Zvieratá", type: "bs" },
  { code: "131", name: "Obstaranie tovaru", type: "bs" },
  { code: "132", name: "Tovar na sklade a v predajniach", type: "bs" },
  { code: "139", name: "Tovar na ceste", type: "bs" },
  { code: "191", name: "Opravná položka k materiálu", type: "bs" },
  { code: "192", name: "Opravná položka k nedokončenej výrobe", type: "bs" },
  { code: "193", name: "Opravná položka k polotovarom vlastnej výroby", type: "bs" },
  { code: "194", name: "Opravná položka k výrobkom", type: "bs" },
  { code: "195", name: "Opravná položka k zvieratám", type: "bs" },
  { code: "196", name: "Opravná položka k tovaru", type: "bs" },

  // Trieda 2 — Finančné účty
  { code: "211", name: "Pokladnica", type: "bs" },
  { code: "213", name: "Ceniny", type: "bs" },
  { code: "221", name: "Bankové účty", type: "bs" },
  { code: "231", name: "Krátkodobé bankové úvery", type: "bs" },
  { code: "232", name: "Eskontné úvery", type: "bs" },
  { code: "241", name: "Vydané krátkodobé dlhopisy", type: "bs" },
  { code: "249", name: "Ostatné krátkodobé finančné výpomoci", type: "bs" },
  { code: "251", name: "Majetkové cenné papiere na obchodovanie", type: "bs" },
  { code: "253", name: "Dlhové cenné papiere na obchodovanie", type: "bs" },
  { code: "255", name: "Vlastné akcie a vlastné obchodné podiely", type: "bs" },
  { code: "256", name: "Dlhové cenné papiere so splatnosťou do jedného roka", type: "bs" },
  { code: "257", name: "Ostatné realizovateľné cenné papiere", type: "bs" },
  { code: "259", name: "Obstaranie krátkodobého finančného majetku", type: "bs" },
  { code: "261", name: "Peniaze na ceste", type: "bs" },
  { code: "291", name: "Opravná položka ku krátkodobému finančnému majetku", type: "bs" },

  // Trieda 3 — Zúčtovacie vzťahy
  { code: "311", name: "Odberatelia", type: "bs" },
  { code: "312", name: "Zmenky na inkaso", type: "bs" },
  { code: "313", name: "Pohľadávky za eskontované cenné papiere", type: "bs" },
  { code: "314", name: "Poskytnuté preddavky", type: "bs" },
  { code: "315", name: "Ostatné pohľadávky", type: "bs" },
  { code: "316", name: "Čistá hodnota zákazky", type: "bs" },
  { code: "321", name: "Dodávatelia", type: "bs" },
  { code: "322", name: "Zmenky na úhradu", type: "bs" },
  { code: "323", name: "Krátkodobé rezervy", type: "bs" },
  { code: "324", name: "Prijaté preddavky", type: "bs" },
  { code: "325", name: "Ostatné záväzky", type: "bs" },
  { code: "326", name: "Nevyfakturované dodávky", type: "bs" },
  { code: "331", name: "Zamestnanci", type: "bs" },
  { code: "333", name: "Ostatné záväzky voči zamestnancom", type: "bs" },
  { code: "335", name: "Pohľadávky voči zamestnancom", type: "bs" },
  { code: "336", name: "Zúčtovanie s orgánmi sociálneho a zdravotného poistenia", type: "bs" },
  { code: "341", name: "Daň z príjmov", type: "bs" },
  { code: "342", name: "Ostatné priame dane", type: "bs" },
  { code: "343", name: "Daň z pridanej hodnoty", type: "bs" },
  { code: "345", name: "Ostatné dane a poplatky", type: "bs" },
  { code: "346", name: "Dotácie zo štátneho rozpočtu", type: "bs" },
  { code: "347", name: "Ostatné dotácie", type: "bs" },
  { code: "351", name: "Pohľadávky voči podnikom v skupine", type: "bs" },
  { code: "352", name: "Pohľadávky voči podnikom s podstatným vplyvom", type: "bs" },
  { code: "353", name: "Pohľadávky za upísané vlastné imanie", type: "bs" },
  { code: "354", name: "Pohľadávky voči spoločníkom a členom pri úhrade straty", type: "bs" },
  { code: "355", name: "Ostatné pohľadávky voči spoločníkom a členom", type: "bs" },
  { code: "358", name: "Pohľadávky voči účastníkom združenia", type: "bs" },
  { code: "361", name: "Záväzky voči podnikom v skupine", type: "bs" },
  { code: "364", name: "Záväzky voči spoločníkom a členom pri rozdelení zisku", type: "bs" },
  { code: "365", name: "Ostatné záväzky voči spoločníkom a členom", type: "bs" },
  { code: "366", name: "Záväzky voči spoločníkom a členom zo závislej činnosti", type: "bs" },
  { code: "367", name: "Záväzky z upísaných nesplatených CP a vkladov", type: "bs" },
  { code: "368", name: "Záväzky voči účastníkom združenia", type: "bs" },
  { code: "371", name: "Pohľadávky z predaja podniku", type: "bs" },
  { code: "372", name: "Záväzky z kúpy podniku", type: "bs" },
  { code: "373", name: "Pohľadávky a záväzky z pevných termínových operácií", type: "bs" },
  { code: "374", name: "Pohľadávky z nájmu", type: "bs" },
  { code: "375", name: "Pohľadávky z vydaných dlhopisov", type: "bs" },
  { code: "376", name: "Nakúpené opcie", type: "bs" },
  { code: "377", name: "Predané opcie", type: "bs" },
  { code: "378", name: "Iné pohľadávky", type: "bs" },
  { code: "379", name: "Iné záväzky", type: "bs" },
  { code: "381", name: "Náklady budúcich období", type: "bs" },
  { code: "382", name: "Komplexné náklady budúcich období", type: "bs" },
  { code: "383", name: "Výdavky budúcich období", type: "bs" },
  { code: "384", name: "Výnosy budúcich období", type: "bs" },
  { code: "385", name: "Príjmy budúcich období", type: "bs" },
  { code: "391", name: "Opravná položka k pohľadávkam", type: "bs" },
  { code: "395", name: "Vnútorné zúčtovanie", type: "bs" },
  { code: "398", name: "Spojovací účet pri združení", type: "bs" },

  // Trieda 4 — Kapitálové účty a dlhodobé záväzky
  { code: "411", name: "Základné imanie", type: "bs" },
  { code: "412", name: "Emisné ážio", type: "bs" },
  { code: "413", name: "Ostatné kapitálové fondy", type: "bs" },
  { code: "414", name: "Oceňovacie rozdiely z precenenia majetku a záväzkov", type: "bs" },
  { code: "415", name: "Oceňovacie rozdiely z kapitálových účastín", type: "bs" },
  { code: "416", name: "Oceňovacie rozdiely z precenenia pri zlúčení a rozdelení", type: "bs" },
  { code: "417", name: "Zákonný rezervný fond z kapitálových vkladov", type: "bs" },
  { code: "418", name: "Nedeliteľný fond z kapitálových vkladov", type: "bs" },
  { code: "419", name: "Zmeny základného imania", type: "bs" },
  { code: "421", name: "Zákonný rezervný fond", type: "bs" },
  { code: "422", name: "Nedeliteľný fond", type: "bs" },
  { code: "423", name: "Štatutárne fondy", type: "bs" },
  { code: "427", name: "Ostatné fondy", type: "bs" },
  { code: "428", name: "Nerozdelený zisk minulých rokov", type: "bs" },
  { code: "429", name: "Neuhradená strata minulých rokov", type: "bs" },
  { code: "431", name: "Výsledok hospodárenia v schvaľovacom konaní", type: "bs" },
  { code: "451", name: "Rezervy zákonné", type: "bs" },
  { code: "459", name: "Ostatné rezervy", type: "bs" },
  { code: "461", name: "Bankové úvery", type: "bs" },
  { code: "471", name: "Dlhodobé záväzky voči podnikom v skupine", type: "bs" },
  { code: "472", name: "Záväzky zo sociálneho fondu", type: "bs" },
  { code: "473", name: "Vydané dlhopisy", type: "bs" },
  { code: "474", name: "Záväzky z nájmu", type: "bs" },
  { code: "475", name: "Dlhodobé prijaté preddavky", type: "bs" },
  { code: "476", name: "Dlhodobé nevyfakturované dodávky", type: "bs" },
  { code: "478", name: "Dlhodobé zmenky na úhradu", type: "bs" },
  { code: "479", name: "Ostatné dlhodobé záväzky", type: "bs" },
  { code: "481", name: "Odložený daňový záväzok a odložená daňová pohľadávka", type: "bs" },
  { code: "491", name: "Vlastné imanie fyzickej osoby – podnikateľa", type: "bs" },

  // Trieda 5 — Náklady
  { code: "501", name: "Spotreba materiálu", type: "pl" },
  { code: "502", name: "Spotreba energie", type: "pl" },
  { code: "503", name: "Spotreba ostatných neskladovateľných dodávok", type: "pl" },
  { code: "504", name: "Predaný tovar", type: "pl" },
  { code: "511", name: "Opravy a udržiavanie", type: "pl" },
  { code: "512", name: "Cestovné", type: "pl" },
  { code: "513", name: "Náklady na reprezentáciu", type: "pl" },
  { code: "518", name: "Ostatné služby", type: "pl" },
  { code: "521", name: "Mzdové náklady", type: "pl" },
  { code: "522", name: "Príjmy spoločníkov a členov zo závislej činnosti", type: "pl" },
  { code: "523", name: "Odmeny členom orgánov spoločnosti a družstva", type: "pl" },
  { code: "524", name: "Zákonné sociálne poistenie", type: "pl" },
  { code: "525", name: "Ostatné sociálne poistenie", type: "pl" },
  { code: "526", name: "Sociálne náklady fyzickej osoby – podnikateľa", type: "pl" },
  { code: "527", name: "Zákonné sociálne náklady", type: "pl" },
  { code: "528", name: "Ostatné sociálne náklady", type: "pl" },
  { code: "531", name: "Daň z motorových vozidiel", type: "pl" },
  { code: "532", name: "Daň z nehnuteľností", type: "pl" },
  { code: "538", name: "Ostatné dane a poplatky", type: "pl" },
  { code: "541", name: "Zostatková cena predaného dlhodobého nehmot. a hmot. majetku", type: "pl" },
  { code: "542", name: "Predaný materiál", type: "pl" },
  { code: "543", name: "Dary", type: "pl" },
  { code: "544", name: "Zmluvné pokuty, penále a úroky z omeškania", type: "pl" },
  { code: "545", name: "Ostatné pokuty, penále a úroky z omeškania", type: "pl" },
  { code: "546", name: "Odpis pohľadávky", type: "pl" },
  { code: "547", name: "Tvorba a zúčtovanie opravných položiek k pohľadávkam", type: "pl" },
  { code: "548", name: "Ostatné náklady na hospodársku činnosť", type: "pl" },
  { code: "549", name: "Manká a škody", type: "pl" },
  { code: "551", name: "Odpisy dlhodobého nehmotného a dlhodobého hmotného majetku", type: "pl" },
  { code: "552", name: "Tvorba a zúčtovanie zákonných rezerv", type: "pl" },
  { code: "553", name: "Tvorba a zúčtovanie ostatných rezerv", type: "pl" },
  { code: "554", name: "Tvorba a zúčtovanie zákonných opravných položiek", type: "pl" },
  { code: "555", name: "Tvorba a zúčtovanie komplexných nákladov budúcich období", type: "pl" },
  { code: "557", name: "Tvorba a zúčtovanie opravných položiek z prevádzkovej činnosti", type: "pl" },
  { code: "558", name: "Tvorba a zúčtovanie zákonných opravných položiek z prev. činnosti", type: "pl" },
  { code: "559", name: "Tvorba a zúčtovanie opravných položiek z finančnej činnosti", type: "pl" },
  { code: "561", name: "Predané cenné papiere a podiely", type: "pl" },
  { code: "562", name: "Úroky", type: "pl" },
  { code: "563", name: "Kurzové straty", type: "pl" },
  { code: "564", name: "Náklady na precenenie cenných papierov", type: "pl" },
  { code: "566", name: "Náklady na krátkodobý finančný majetok", type: "pl" },
  { code: "567", name: "Náklady na derivátové operácie", type: "pl" },
  { code: "568", name: "Ostatné finančné náklady", type: "pl" },
  { code: "569", name: "Manká a škody na finančnom majetku", type: "pl" },
  { code: "591", name: "Splatná daň z príjmov z bežnej činnosti", type: "pl" },
  { code: "592", name: "Odložená daň z príjmov z bežnej činnosti", type: "pl" },
  { code: "593", name: "Splatná daň z príjmov z mimoriadnej činnosti", type: "pl" },
  { code: "594", name: "Odložená daň z príjmov z mimoriadnej činnosti", type: "pl" },
  { code: "595", name: "Dodatočné odvody dane z príjmov", type: "pl" },
  { code: "596", name: "Prevod podielov na výsledku hospodárenia spoločníkom", type: "pl" },

  // Trieda 6 — Výnosy
  { code: "601", name: "Tržby za vlastné výrobky", type: "pl" },
  { code: "602", name: "Tržby z predaja služieb", type: "pl" },
  { code: "604", name: "Tržby za tovar", type: "pl" },
  { code: "606", name: "Výnosy zo zákazky", type: "pl" },
  { code: "607", name: "Výnosy zo zhotovenia nehnuteľnosti na predaj", type: "pl" },
  { code: "611", name: "Zmena stavu nedokončenej výroby", type: "pl" },
  { code: "612", name: "Zmena stavu polotovarov vlastnej výroby", type: "pl" },
  { code: "613", name: "Zmena stavu výrobkov", type: "pl" },
  { code: "614", name: "Zmena stavu zvierat", type: "pl" },
  { code: "621", name: "Aktivácia materiálu a tovaru", type: "pl" },
  { code: "622", name: "Aktivácia vnútroorganizačných služieb", type: "pl" },
  { code: "623", name: "Aktivácia dlhodobého nehmotného majetku", type: "pl" },
  { code: "624", name: "Aktivácia dlhodobého hmotného majetku", type: "pl" },
  { code: "641", name: "Tržby z predaja dlhodobého nehmotného a hmotného majetku", type: "pl" },
  { code: "642", name: "Tržby z predaja materiálu", type: "pl" },
  { code: "644", name: "Zmluvné pokuty, penále a úroky z omeškania", type: "pl" },
  { code: "645", name: "Ostatné pokuty, penále a úroky z omeškania", type: "pl" },
  { code: "646", name: "Výnosy z odpísaných pohľadávok", type: "pl" },
  { code: "648", name: "Ostatné výnosy z hospodárskej činnosti", type: "pl" },
  { code: "655", name: "Zúčtovanie komplexných nákladov budúcich období", type: "pl" },
  { code: "657", name: "Zúčtovanie opravných položiek z prevádzkovej činnosti", type: "pl" },
  { code: "658", name: "Zúčtovanie zákonných opravných položiek z prev. činnosti", type: "pl" },
  { code: "659", name: "Zúčtovanie opravných položiek z finančnej činnosti", type: "pl" },
  { code: "661", name: "Tržby z predaja cenných papierov a podielov", type: "pl" },
  { code: "662", name: "Úroky", type: "pl" },
  { code: "663", name: "Kurzové zisky", type: "pl" },
  { code: "664", name: "Výnosy z precenenia cenných papierov", type: "pl" },
  { code: "665", name: "Výnosy z dlhodobého finančného majetku", type: "pl" },
  { code: "666", name: "Výnosy z krátkodobého finančného majetku", type: "pl" },
  { code: "667", name: "Výnosy z derivátových operácií", type: "pl" },
  { code: "668", name: "Ostatné finančné výnosy", type: "pl" },
  { code: "698", name: "Prevod podielov na výsledku hospodárenia spoločníkom", type: "pl" },
];

// BalanceSheetGroup / ProfitAndLossStatementGroup definitions. Groups are matched
// by name on re-runs so repeated installs don't duplicate them.
//
// Balance sheet groups are parented under the singleton Assets, Liabilities, or
// Equity groups (fixed Guids in PARENT_KEYS). Mixed classes (2, 3, 4) are split
// by code so every group has a single correct parent.
//
// ProfitAndLossStatementGroupType: 0 = IncomeGroup, 1 = ExpenseGroup, 2 = SubgroupOf.

const PARENT_KEYS = {
  assets:      "4c05c221-ca57-4c7c-be62-115669302ed4",
  liabilities: "ed5a19f6-12c5-45cc-b4b7-4e79f7ef50bc",
  equity:      "9275ff4c-4cff-41d0-b7b5-f31c783f03d8",
};

// Position values are per-parent and follow Slovak balance-sheet convention:
// non-current → current on the asset side, long-term → short-term on the liability side.
// Equity accounts (class 4: 411–431, 491) are not grouped — they sit directly under
// the built-in Equity singleton, since Manager's Equity section already groups them.
const BS_GROUPS = {
  // under Aktíva
  dlhodobyMajetok:  { name: "Dlhodobý majetok",                             parent: "assets",      position: 1 },
  zasoby:           { name: "Zásoby",                                       parent: "assets",      position: 2 },
  pohladavky:       { name: "Pohľadávky",                                   parent: "assets",      position: 3 },
  financneUcty:     { name: "Finančné účty",                                parent: "assets",      position: 4 },
  // under Pasíva (Záväzky)
  dlhodobeZavazky:  { name: "Dlhodobé záväzky a rezervy",                   parent: "liabilities", position: 1 },
  zavazky:          { name: "Záväzky",                                      parent: "liabilities", position: 2 },
  kratkodobeUvery:  { name: "Krátkodobé bankové úvery a finančné výpomoci", parent: "liabilities", position: 3 },
};

const PL_GROUPS = {
  vynosy:  { name: "Výnosy",  plType: 0, position: 1 },
  naklady: { name: "Náklady", plType: 1, position: 2 },
};

const CLASS_LABELS = {
  "0": "0 — Dlhodobý majetok",
  "1": "1 — Zásoby",
  "2": "2 — Finančné účty",
  "3": "3 — Zúčtovacie vzťahy",
  "4": "4 — Kapitálové účty a dlhodobé záväzky",
  "5": "5 — Náklady",
  "6": "6 — Výnosy",
};

// Within class 2, these synthetic accounts represent short-term loans/borrowings
// (liabilities); everything else in class 2 is an asset (cash, bank, securities).
const SHORT_TERM_LOAN_2X = new Set(["231", "232", "241", "249"]);

// Within class 3, these synthetic accounts are receivables (assets). Everything
// else in class 3 is treated as a payable (liability) for grouping purposes —
// the user can re-classify edge cases by hand afterwards.
const RECEIVABLES_3X = new Set([
  "311", "312", "313", "314", "315", "316",  // odberatelia, zmenky, preddavky
  "335",                                       // pohľadávky voči zamestnancom
  "351", "352", "353", "354", "355", "358",    // pohľadávky voči skupine a spoločníkom
  "371", "373", "374", "375", "376", "378",    // iné pohľadávky
  "381", "382", "385",                         // časové rozlíšenie aktív
  "391", "395", "398",                         // opravná položka, vnútorné zúčtovanie, spoj. účet
]);

// Within class 4, equity accounts are 411–431 (základné imanie, kapitálové fondy,
// fondy zo zisku, nerozdelený zisk / strata, výsledok hospodárenia) plus 491
// (vlastné imanie fyzickej osoby – podnikateľa). The rest of class 4 (451–481)
// is long-term liabilities and provisions.
function isEquity4X(code) {
  if (code === "491") return true;
  const prefix = code.substring(0, 2);
  return prefix === "41" || prefix === "42" || prefix === "43";
}

// Returns either a BS_GROUPS/PL_GROUPS key (intermediate group will be created /
// reused) or a PARENT_KEYS key (account is parented directly under the singleton —
// used for equity, which Manager already groups under its built-in Equity section).
function groupKeyFor(tpl) {
  if (tpl.type === "pl") {
    return tpl.code.charAt(0) === "5" ? "naklady" : "vynosy";
  }
  const c = tpl.code.charAt(0);
  if (c === "0") return "dlhodobyMajetok";
  if (c === "1") return "zasoby";
  if (c === "2") return SHORT_TERM_LOAN_2X.has(tpl.code) ? "kratkodobeUvery" : "financneUcty";
  if (c === "3") return RECEIVABLES_3X.has(tpl.code) ? "pohladavky" : "zavazky";
  if (c === "4") return isEquity4X(tpl.code) ? "equity" : "dlhodobeZavazky";
  return null;
}

// ---------------------------------------------------------------------------

(function initCoaTab() {
  const coaView = document.getElementById("coa-view");
  if (!coaView) return;

  const businessSel = document.getElementById("business");
  const output = document.getElementById("coaOutput");
  const refreshBtn = document.getElementById("refreshCoa");
  const installAllBtn = document.getElementById("installAllCoa");

  let existing = { bs: [], pl: [], bsGroups: [], plGroups: [] };
  // Cache of resolved group Guids by group-key (e.g. "pohladavky"), so a bulk
  // install creates each group at most once.
  let groupKeyToGuid = {};

  function trimLower(s) { return (s || "").trim().toLowerCase(); }
  function escText(s) { return String(s).replace(/&/g, "&amp;").replace(/</g, "&lt;"); }

  function findExisting(template) {
    const list = template.type === "bs" ? existing.bs : existing.pl;
    return list.find(it => trimLower(it.value.code) === trimLower(template.code)) || null;
  }

  function findExistingGroup(name, type) {
    const list = type === "bs" ? existing.bsGroups : existing.plGroups;
    const target = trimLower(name);
    return list.find(it => trimLower(it.value.name) === target) || null;
  }

  async function loadCoa() {
    const business = businessSel.value;
    if (!business) { output.innerHTML = `<div class="error">Vyberte podnik.</div>`; return; }
    output.innerHTML = `<div class="status">Načítavam účty…</div>`;
    groupKeyToGuid = {};
    try {
      const [bsRes, plRes, bsGroupRes, plGroupRes] = await Promise.all([
        fetch(`/api4/balance-sheet-account-batch?business=${encodeURIComponent(business)}&pageSize=1000`, { credentials: "include" }),
        fetch(`/api4/profit-and-loss-statement-account-batch?business=${encodeURIComponent(business)}&pageSize=1000`, { credentials: "include" }),
        fetch(`/api4/balance-sheet-group-batch?business=${encodeURIComponent(business)}&pageSize=1000`, { credentials: "include" }),
        fetch(`/api4/profit-and-loss-statement-group-batch?business=${encodeURIComponent(business)}&pageSize=1000`, { credentials: "include" }),
      ]);
      if (!bsRes.ok) throw new Error(`Balance sheet accounts: HTTP ${bsRes.status}`);
      if (!plRes.ok) throw new Error(`Profit & loss accounts: HTTP ${plRes.status}`);
      if (!bsGroupRes.ok) throw new Error(`Balance sheet groups: HTTP ${bsGroupRes.status}`);
      if (!plGroupRes.ok) throw new Error(`Profit & loss groups: HTTP ${plGroupRes.status}`);
      const bsData = await bsRes.json();
      const plData = await plRes.json();
      const bsGroupData = await bsGroupRes.json();
      const plGroupData = await plGroupRes.json();
      existing.bs = (bsData.items || []).map(it => ({ key: it.key, value: it.item || {} }));
      existing.pl = (plData.items || []).map(it => ({ key: it.key, value: it.item || {} }));
      existing.bsGroups = (bsGroupData.items || []).map(it => ({ key: it.key, value: it.item || {} }));
      existing.plGroups = (plGroupData.items || []).map(it => ({ key: it.key, value: it.item || {} }));
      // Pre-populate the cache from any matching groups that already exist.
      for (const key of Object.keys(BS_GROUPS)) {
        const m = findExistingGroup(BS_GROUPS[key].name, "bs");
        if (m) groupKeyToGuid[key] = m.key;
      }
      for (const key of Object.keys(PL_GROUPS)) {
        const m = findExistingGroup(PL_GROUPS[key].name, "pl");
        if (m) groupKeyToGuid[key] = m.key;
      }
      renderCoa();
    } catch (err) {
      output.innerHTML = `<div class="error">Nepodarilo sa načítať účty: ${err.message}</div>`;
    }
  }

  function classLabel(code) {
    return CLASS_LABELS[code.charAt(0)] || code.charAt(0);
  }

  function renderCoa() {
    const business = businessSel.value;
    if (!business) { output.innerHTML = `<div class="error">Vyberte podnik.</div>`; return; }

    const groups = new Map();
    for (let i = 0; i < COA_TEMPLATE.length; i++) {
      const tpl = COA_TEMPLATE[i];
      const cls = tpl.code.charAt(0);
      if (!groups.has(cls)) groups.set(cls, []);
      groups.get(cls).push({ tpl, idx: i });
    }

    let html = `<table>
      <thead><tr><th>Kód</th><th>Názov</th><th>Typ</th><th>Stav</th><th>Akcia</th></tr></thead><tbody>`;
    let missingCount = 0;
    for (const [cls, rows] of [...groups.entries()].sort()) {
      html += `<tr><th colspan="5">${escText(classLabel(rows[0].tpl.code))}</th></tr>`;
      for (const { tpl, idx } of rows) {
        const match = findExisting(tpl);
        let badge, action;
        if (match) {
          badge = `<span class="badge ok">Existuje</span> <span class="muted">→ ${escText(match.value.name || "")}</span>`;
          action = `<button class="secondary" disabled>OK</button>`;
        } else {
          missingCount++;
          badge = `<span class="badge missing">Chýba</span>`;
          action = `<button class="secondary" data-action="create" data-idx="${idx}">Vytvoriť</button>`;
        }
        html += `<tr>
          <td><strong>${escText(tpl.code)}</strong></td>
          <td>${escText(tpl.name)}</td>
          <td>${tpl.type === "bs" ? "Súvaha" : "Výsledovka"}</td>
          <td>${badge}</td>
          <td>${action}</td>
        </tr>`;
      }
    }
    html += `</tbody></table>`;
    output.innerHTML = html;

    installAllBtn.disabled = missingCount === 0;
    installAllBtn.textContent = missingCount === 0
      ? "Všetky účty existujú"
      : `Vytvoriť všetky chýbajúce (${missingCount})`;

    for (const btn of output.querySelectorAll("button[data-action='create']")) {
      btn.addEventListener("click", () => onCreate(btn));
    }
  }

  function accountEndpointFor(type) {
    return type === "bs" ? "/api4/balance-sheet-account" : "/api4/profit-and-loss-statement-account";
  }

  function groupEndpointFor(type) {
    return type === "bs" ? "/api4/balance-sheet-group" : "/api4/profit-and-loss-statement-group";
  }

  async function ensureGroup(key) {
    if (groupKeyToGuid[key]) return groupKeyToGuid[key];

    const bsDef = BS_GROUPS[key];
    const plDef = PL_GROUPS[key];
    const def = bsDef || plDef;
    if (!def) throw new Error(`Unknown group '${key}'`);
    const type = bsDef ? "bs" : "pl";

    const business = businessSel.value;
    const value = { Name: def.name, Position: def.position };
    if (bsDef) value.Group = PARENT_KEYS[def.parent];
    else value.Type = def.plType;

    const res = await fetch(groupEndpointFor(type), {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ Business: business, Value: value }),
      credentials: "include",
    });
    if (!res.ok) throw new Error(`Skupina "${def.name}": HTTP ${res.status}: ${await res.text()}`);
    const guid = await res.json();
    groupKeyToGuid[key] = guid;
    return guid;
  }

  async function createOne(tpl) {
    const business = businessSel.value;
    const groupKey = groupKeyFor(tpl);
    if (!groupKey) throw new Error(`Cannot determine group for account ${tpl.code}`);
    // Equity accounts go directly under the built-in Equity singleton; everything
    // else gets a custom intermediate group (created on first use, then cached).
    const groupGuid = (groupKey in PARENT_KEYS)
      ? PARENT_KEYS[groupKey]
      : await ensureGroup(groupKey);
    const value = { Name: tpl.name, Code: tpl.code, Group: groupGuid };
    const res = await fetch(accountEndpointFor(tpl.type), {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ Business: business, Value: value }),
      credentials: "include",
    });
    if (!res.ok) throw new Error(`HTTP ${res.status}: ${await res.text()}`);
  }

  async function onCreate(btn) {
    const idx = parseInt(btn.dataset.idx, 10);
    const tpl = COA_TEMPLATE[idx];
    btn.disabled = true;
    btn.textContent = "Vytváram…";
    try {
      await createOne(tpl);
      await loadCoa();
    } catch (err) {
      btn.disabled = false;
      btn.textContent = "Vytvoriť";
      alert(`Vytvorenie zlyhalo: ${err.message}`);
    }
  }

  async function onInstallAll() {
    const business = businessSel.value;
    if (!business) return;
    const missing = COA_TEMPLATE.filter(t => !findExisting(t));
    if (missing.length === 0) return;

    installAllBtn.disabled = true;

    // 1) Resolve every intermediate group needed for this batch, in parallel.
    //    ensureGroup is cached by key, so each group is POSTed at most once.
    //    Accounts whose groupKey is in PARENT_KEYS (equity) use the singleton directly.
    installAllBtn.textContent = "Pripravujem skupiny…";
    const groupKeysNeeded = new Set();
    for (const tpl of missing) {
      const k = groupKeyFor(tpl);
      if (k && !(k in PARENT_KEYS)) groupKeysNeeded.add(k);
    }
    await Promise.all([...groupKeysNeeded].map(k =>
      ensureGroup(k).catch(err => { console.error(`Group "${k}" failed:`, err); })
    ));

    // 2) Bucket missing accounts by type; drop any whose group couldn't be resolved.
    const bsAccounts = [];
    const plAccounts = [];
    let failed = 0;
    for (const tpl of missing) {
      const k = groupKeyFor(tpl);
      const groupGuid = !k ? null
        : (k in PARENT_KEYS) ? PARENT_KEYS[k]
        : groupKeyToGuid[k];
      if (!groupGuid) { failed++; continue; }
      const value = { Name: tpl.name, Code: tpl.code, Group: groupGuid };
      if (tpl.type === "bs") bsAccounts.push(value);
      else plAccounts.push(value);
    }

    // 3) One batch POST per account type, run in parallel.
    installAllBtn.textContent = `Vytváram účty (${bsAccounts.length + plAccounts.length})…`;
    let created = 0;
    async function postBatch(endpoint, values, label) {
      if (values.length === 0) return;
      try {
        const res = await fetch(endpoint, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ Business: business, Values: values }),
          credentials: "include",
        });
        if (!res.ok) throw new Error(`HTTP ${res.status}: ${await res.text()}`);
        const keys = await res.json();
        created += (keys || []).length;
      } catch (err) {
        console.error(`${label} batch failed:`, err);
        failed += values.length;
      }
    }
    await Promise.all([
      postBatch("/api4/balance-sheet-account-batch", bsAccounts, "BS"),
      postBatch("/api4/profit-and-loss-statement-account-batch", plAccounts, "PL"),
    ]);

    await loadCoa();
    if (failed > 0) alert(`Hotovo. Vytvorené: ${created}, zlyhalo: ${failed}. Pozri konzolu prehliadača.`);
  }

  refreshBtn.addEventListener("click", loadCoa);
  installAllBtn.addEventListener("click", onInstallAll);
  businessSel.addEventListener("change", () => {
    if (businessSel.value) loadCoa();
    else output.innerHTML = "";
  });

  // app.js may have already populated and selected a business by now; if so, load immediately.
  if (businessSel.value) loadCoa();
})();
