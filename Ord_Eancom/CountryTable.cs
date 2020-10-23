using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ord_Eancom
{
    public class CountryTable
    {
        public CountryTable()
        {
        }

        public static string GetTwoLetterISO(string country)
        {
            switch (country)
            {
                //case "Afghanistan":
                case "Afghanistan":
                    return "AF";
                case "Albanie":
                case "Albania":
                    return "AL";
                case "Antarctique":
                case "Antarctica":
                    return "AQ";
                case "Algérie":
                case "Algeria":
                    return "DZ";
                case "Samoa Américaines":
                case "American Samoa":
                    return "AS";
                case "Andorre":
                case "Andorra":
                    return "AD";
                //case "Angola":
                case "Angola":
                    return "AO";
                case "Antigua-et-Barbuda":
                case "Antigua and Barbuda":
                    return "AG";
                case "Azerbaïdjan":
                case "Azerbaijan":
                    return "AZ";
                case "Argentine":
                case "Argentina":
                    return "AR";
                case "Australie":
                case "Australia":
                    return "AU";
                case "Autriche":
                case "Austria":
                    return "AT";
                //case "Bahamas":
                case "Bahamas":
                    return "BS";
                case "Bahreïn":
                case "Bahrain":
                    return "BH";
                //case "Bangladesh":
                case "Bangladesh":
                    return "BD";
                case "Arménie":
                case "Armenia":
                    return "AM";
                case "Barbade":
                case "Barbados":
                    return "BB";
                case "Belgique":
                case "Belgium":
                    return "BE";
                case "Bermudes":
                case "Bermuda":
                    return "BM";
                case "Bhoutan":
                case "Bhutan":
                    return "BT";
                case "Bolivie":
                case "Bolivia":
                    return "BO";
                case "Bosnie-Herzégovine":
                case "Bosnia and Herzegovina":
                    return "BA";
                //case "Botswana":
                case "Botswana":
                    return "BW";
                case "Île Bouvet":
                case "Bouvet Island":
                    return "BV";
                case "Brésil":
                case "Brazil":
                    return "BR";
                //case "Belize":
                case "Belize":
                    return "BZ";
                case "Territoire Britannique de l'Océan Indien":
                case "British Indian Ocean Territory":
                    return "IO";
                case "Îles Salomon":
                case "Solomon Islands":
                    return "SB";
                case "Îles Vierges Britanniques":
                case "British Virgin Islands":
                    return "VG";
                case "Brunéi Darussalam":
                case "Brunei Darussalam":
                    return "BN";
                case "Bulgarie":
                case "Bulgaria":
                    return "BG";
                //case "Myanmar":
                case "Myanmar":
                    return "MM";
                //case "Burundi":
                case "Burundi":
                    return "BI";
                case "Bélarus":
                case "Belarus":
                    return "BY";
                case "Cambodge":
                case "Cambodia":
                    return "KH";
                case "Cameroun":
                case "Cameroon":
                    return "CM";
                //case "Canada":
                case "Canada":
                    return "CA";
                case "Cap-vert":
                case "Cape Verde":
                    return "CV";
                case "Îles Caïmanes":
                case "Cayman Islands":
                    return "KY";
                case "République Centrafricaine":
                case "Central African":
                    return "CF";
                //case "Sri Lanka":
                case "Sri Lanka":
                    return "LK";
                case "Tchad":
                case "Chad":
                    return "TD";
                case "Chili":
                case "Chile":
                    return "CL";
                case "Chine":
                case "China":
                    return "CN";
                case "Taïwan":
                case "Taiwan":
                    return "TW";
                case "Île Christmas":
                case "Christmas Island":
                    return "CX";
                case "Îles Cocos (Keeling)":
                case "Cocos (Keeling) Islands":
                    return "CC";
                case "Colombie":
                case "Colombia":
                    return "CO";
                case "Comores":
                case "Comoros":
                    return "KM";
                //case "Mayotte":
                case "Mayotte":
                    return "YT";
                case "République du Congo":
                case "Republic of the Congo":
                    return "CG";
                case "République Démocratique du Congo":
                case "The Democratic Republic Of The Congo":
                    return "CD";
                case "Îles Cook":
                case "Cook Islands":
                    return "CK";
                //case "Costa Rica":
                case "Costa Rica":
                    return "CR";
                case "Croatie":
                case "Croatia":
                    return "HR";
                //case "Cuba":
                case "Cuba":
                    return "CU";
                case "Chypre":
                case "Cyprus":
                    return "CY";
                case "République Tchèque":
                case "Czech Republic":
                    return "CZ";
                case "Bénin":
                case "Benin":
                    return "BJ";
                case "Danemark":
                case "Denmark":
                    return "DK";
                case "Dominique":
                case "Dominica":
                    return "DM";
                case "République Dominicaine":
                case "Dominican Republic":
                    return "DO";
                case "Équateur":
                case "Ecuador":
                    return "EC";
                //case "El Salvador":
                case "El Salvador":
                    return "SV";
                case "Guinée Équatoriale":
                case "Equatorial Guinea":
                    return "GQ";
                case "Éthiopie":
                case "Ethiopia":
                    return "ET";
                case "Érythrée":
                case "Eritrea":
                    return "ER";
                case "Estonie":
                case "Estonia":
                    return "EE";
                case "Îles Féroé":
                case "Faroe Islands":
                    return "FO";
                case "Îles (malvinas) Falkland":
                case "Falkland Islands":
                    return "FK";
                case "Géorgie du Sud et les Îles Sandwich du Sud":
                case "South Georgia and the South Sandwich Islands":
                    return "GS";
                case "Fidji":
                case "Fiji":
                    return "FJ";
                case "Finlande":
                case "Finland":
                    return "FI";
                case "Îles Åland":
                case "Åland Islands":
                    return "AX";
                //case "France":
                case "France":
                    return "FR";
                case "Guyane Française":
                case "French Guiana":
                    return "GF";
                case "Polynésie Française":
                case "French Polynesia":
                    return "PF";
                case "Terres Australes Françaises":
                case "French Southern Territories":
                    return "TF";
                //case "Djibouti":
                case "Djibouti":
                    return "DJ";
                //case "Gabon":
                case "Gabon":
                    return "GA";
                case "Géorgie":
                case "Georgia":
                    return "GE";
                case "Gambie":
                case "Gambia":
                    return "GM";
                case "Territoire Palestinien Occupé":
                case "Occupied Palestinian Territory":
                    return "PS";
                case "Allemagne":
                case "Germany":
                    return "DE";
                //case "Ghana":
                case "Ghana":
                    return "GH";
                //case "Gibraltar":
                case "Gibraltar":
                    return "GI";
                //case "Kiribati":
                case "Kiribati":
                    return "KI";
                case "Grèce":
                case "Greece":
                    return "GR";
                case "Groenland":
                case "Greenland":
                    return "GL";
                case "Grenade":
                case "Grenada":
                    return "GD";
                //case "Guadeloupe":
                case "Guadeloupe":
                    return "GP";
                //case "Guam":
                case "Guam":
                    return "GU";
                //case "Guatemala":
                case "Guatemala":
                    return "GT";
                case "Guinée":
                case "Guinea":
                    return "GN";
                //case "Guyana":
                case "Guyana":
                    return "GY";
                case "Haïti":
                case "Haiti":
                    return "HT";
                case "Îles Heard et Mcdonald":
                case "Heard Island and McDonald Islands":
                    return "HM";
                case "Saint-Siège (état de la Cité du Vatican)":
                case "Vatican City State":
                    return "VA";
                //case "Honduras":
                case "Honduras":
                    return "HN";
                case "Hong-Kong":
                case "Hong Kong":
                    return "HK";
                case "Hongrie":
                case "Hungary":
                    return "HU";
                case "Islande":
                case "Iceland":
                    return "IS";
                case "Inde":
                case "India":
                    return "IN";
                case "Indonésie":
                case "Indonesia":
                    return "ID";
                case "République Islamique d'Iran":
                case "Islamic Republic of Iran":
                    return "IR";
                //case "Iraq":
                case "Iraq":
                    return "IQ";
                case "Irlande":
                case "Ireland":
                    return "IE";
                case "Israël":
                case "Israel":
                    return "IL";
                case "Italie":
                case "Italy":
                    return "IT";
                //case "Côte d'Ivoire":
                case "Côte d'Ivoire":
                    return "CI";
                case "Jamaïque":
                case "Jamaica":
                    return "JM";
                case "Japon":
                case "Japan":
                    return "JP";
                //case "Kazakhstan":
                case "Kazakhstan":
                    return "KZ";
                case "Jordanie":
                case "Jordan":
                    return "JO";
                //case "Kenya":
                case "Kenya":
                    return "KE";
                case "République Populaire Démocratique de Corée":
                case "Democratic People's Republic of Korea":
                    return "KP";
                case "République de Corée":
                case "Republic of Korea":
                    return "KR";
                case "Koweït":
                case "Kuwait":
                    return "KW";
                case "Kirghizistan":
                case "Kyrgyzstan":
                    return "KG";
                case "République Démocratique Populaire Lao":
                case "Lao People's Democratic Republic":
                    return "LA";
                case "Liban":
                case "Lebanon":
                    return "LB";
                //case "Lesotho":
                case "Lesotho":
                    return "LS";
                case "Lettonie":
                case "Latvia":
                    return "LV";
                case "Libéria":
                case "Liberia":
                    return "LR";
                case "Jamahiriya Arabe Libyenne":
                case "Libyan Arab Jamahiriya":
                    return "LY";
                //case "Liechtenstein":
                case "Liechtenstein":
                    return "LI";
                case "Lituanie":
                case "Lithuania":
                    return "LT";
                //case "Luxembourg":
                case "Luxembourg":
                    return "LU";
                //case "Macao":
                case "Macao":
                    return "MO";
                //case "Madagascar":
                case "Madagascar":
                    return "MG";
                //case "Malawi":
                case "Malawi":
                    return "MW";
                case "Malaisie":
                case "Malaysia":
                    return "MY";
                //case "Maldives":
                case "Maldives":
                    return "MV";
                //case "Mali":
                case "Mali":
                    return "ML";
                case "Malte":
                case "Malta":
                    return "MT";
                //case "Martinique":
                case "Martinique":
                    return "MQ";
                case "Mauritanie":
                case "Mauritania":
                    return "MR";
                case "Maurice":
                case "Mauritius":
                    return "MU";
                case "Mexique":
                case "Mexico":
                    return "MX";
                //case "Monaco":
                case "Monaco":
                    return "MC";
                case "Mongolie":
                case "Mongolia":
                    return "MN";
                case "République de Moldova":
                case "Republic of Moldova":
                    return "MD";
                //case "Montserrat":
                case "Montserrat":
                    return "MS";
                case "Maroc":
                case "Morocco":
                    return "MA";
                //case "Mozambique":
                case "Mozambique":
                    return "MZ";
                //case "Oman":
                case "Oman":
                    return "OM";
                case "Namibie":
                case "Namibia":
                    return "NA";
                //case "Nauru":
                case "Nauru":
                    return "NR";
                case "Népal":
                case "Nepal":
                    return "NP";
                case "Pays-Bas":
                case "Netherlands":
                    return "NL";
                case "Antilles Néerlandaises":
                case "Netherlands Antilles":
                    return "AN";
                //case "Aruba":
                case "Aruba":
                    return "AW";
                case "Nouvelle-Calédonie":
                case "New Caledonia":
                    return "NC";
                //case "Vanuatu":
                case "Vanuatu":
                    return "VU";
                case "Nouvelle-Zélande":
                case "New Zealand":
                    return "NZ";
                //case "Nicaragua":
                case "Nicaragua":
                    return "NI";
                //case "Niger":
                case "Niger":
                    return "NE";
                case "Nigéria":
                case "Nigeria":
                    return "NG";
                case "Niué":
                case "Niue":
                    return "NU";
                case "Île Norfolk":
                case "Norfolk Island":
                    return "NF";
                case "Norvège":
                case "Norway":
                    return "NO";
                case "Îles Mariannes du Nord":
                case "Northern Mariana Islands":
                    return "MP";
                case "Îles Mineures Éloignées des États-Unis":
                case "United States Minor Outlying Islands":
                    return "UM";
                case "États Fédérés de Micronésie":
                case "Federated States of Micronesia":
                    return "FM";
                case "Îles Marshall":
                case "Marshall Islands":
                    return "MH";
                case "Palaos":
                case "Palau":
                    return "PW";
                //case "Pakistan":
                case "Pakistan":
                    return "PK";
                //case "Panama":
                case "Panama":
                    return "PA";
                case "Papouasie-Nouvelle-Guinée":
                case "Papua New Guinea":
                    return "PG";
                //case "Paraguay":
                case "Paraguay":
                    return "PY";
                case "Pérou":
                case "Peru":
                    return "PE";
                //case "Philippines":
                case "Philippines":
                    return "PH";
                //case "Pitcairn":
                case "Pitcairn":
                    return "PN";
                case "Pologne":
                case "Poland":
                    return "PL";
                //case "Portugal":
                case "Portugal":
                    return "PT";
                case "Guinée-Bissau":
                case "Guinea-Bissau":
                    return "GW";
                //case "Timor-Leste":
                case "Timor-Leste":
                    return "TL";
                case "Porto Rico":
                case "Puerto Rico":
                    return "PR";
                //case "Qatar":
                case "Qatar":
                    return "QA";
                //case "Réunion":
                case "Réunion":
                    return "RE";
                case "Roumanie":
                case "Romania":
                    return "RO";
                case "Fédération de Russie":
                case "Russian Federation":
                    return "RU";
                //case "Rwanda":
                case "Rwanda":
                    return "RW";
                case "Sainte-Hélène":
                case "Saint Helena":
                    return "SH";
                case "Saint-Kitts-et-Nevis":
                case "Saint Kitts and Nevis":
                    return "KN";
                //case "Anguilla":
                case "Anguilla":
                    return "AI";
                case "Sainte-Lucie":
                case "Saint Lucia":
                    return "LC";
                case "Saint-Pierre-et-Miquelon":
                case "Saint-Pierre and Miquelon":
                    return "PM";
                case "Saint-Vincent-et-les Grenadines":
                case "Saint Vincent and the Grenadines":
                    return "VC";
                case "Saint-Marin":
                case "San Marino":
                    return "SM";
                case "Sao Tomé-et-Principe":
                case "Sao Tome and Principe":
                    return "ST";
                case "Arabie Saoudite":
                case "Saudi Arabia":
                    return "SA";
                case "Sénégal":
                case "Senegal":
                    return "SN";
                //case "Seychelles":
                case "Seychelles":
                    return "SC";
                //case "Sierra Leone":
                case "Sierra Leone":
                    return "SL";
                case "Singapour":
                case "Singapore":
                    return "SG";
                case "Slovaquie":
                case "Slovakia":
                    return "SK";
                case "Viet Nam":
                case "Vietnam":
                    return "VN";
                case "Slovénie":
                case "Slovenia":
                    return "SI";
                case "Somalie":
                case "Somalia":
                    return "SO";
                case "Afrique du Sud":
                case "South Africa":
                    return "ZA";
                //case "Zimbabwe":
                case "Zimbabwe":
                    return "ZW";
                case "Espagne":
                case "Spain":
                    return "ES";
                case "Sahara Occidental":
                case "Western Sahara":
                    return "EH";
                case "Soudan":
                case "Sudan":
                    return "SD";
                //case "Suriname":
                case "Suriname":
                    return "SR";
                case "Svalbard etÎle Jan Mayen":
                case "Svalbard and Jan Mayen":
                    return "SJ";
                //case "Swaziland":
                case "Swaziland":
                    return "SZ";
                case "Suède":
                case "Sweden":
                    return "SE";
                case "Suisse":
                case "Switzerland":
                    return "CH";
                case "République Arabe Syrienne":
                case "Syrian Arab Republic":
                    return "SY";
                case "Tadjikistan":
                case "Tajikistan":
                    return "TJ";
                case "Thaïlande":
                case "Thailand":
                    return "TH";
                //case "Togo":
                case "Togo":
                    return "TG";
                //case "Tokelau":
                case "Tokelau":
                    return "TK";
                //case "Tonga":
                case "Tonga":
                    return "TO";
                case "Trinité-et-Tobago":
                case "Trinidad and Tobago":
                    return "TT";
                case "Émirats Arabes Unis":
                case "United Arab Emirates":
                    return "AE";
                case "Tunisie":
                case "Tunisia":
                    return "TN";
                case "Turquie":
                case "Turkey":
                    return "TR";
                case "Turkménistan":
                case "Turkmenistan":
                    return "TM";
                case "Îles Turks et Caïques":
                case "Turks and Caicos Islands":
                    return "TC";
                //case "Tuvalu":
                case "Tuvalu":
                    return "TV";
                case "Ouganda":
                case "Uganda":
                    return "UG";
                //case "Ukraine":
                case "Ukraine":
                    return "UA";
                case "L'ex-République Yougoslave de Macédoine":
                case "The Former Yugoslav Republic of Macedonia":
                    return "MK";
                case "Égypte":
                case "Egypt":
                    return "EG";
                case "Royaume-Uni":
                case "United Kingdom":
                    return "GB";
                case "Île de Man":
                case "Isle of Man":
                    return "IM";
                case "République-Unie de Tanzanie":
                case "United Republic Of Tanzania":
                    return "TZ";
                case "États-Unis":
                case "United States":
                    return "US";
                case "Îles Vierges des États-Unis":
                case "U.S. Virgin Islands":
                    return "VI";
                //case "Burkina Faso":
                case "Burkina Faso":
                    return "BF";
                //case "Uruguay":
                case "Uruguay":
                    return "UY";
                case "Ouzbékistan":
                case "Uzbekistan":
                    return "UZ";
                //case "Venezuela":
                case "Venezuela":
                    return "VE";
                case "Wallis et Futuna":
                case "Wallis and Futuna":
                    return "WF";
                //case "Samoa":
                case "Samoa":
                    return "WS";
                case "Yémen":
                case "Yemen":
                    return "YE";
                case "Serbie-et-Monténégro":
                case "Serbia and Montenegro":
                    return "CS";
                case "Zambie":
                case "Zambia":
                    return "ZM";
                default:
                    return String.Empty;                    
            }
        }
    }
}
