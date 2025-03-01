// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Internal.Filters
{
    // Filter XML formats
    //
    // Empty Filter:
    //    <filter/>
    //
    // Id Filter:
    //    <id>1</id>
    //    <id>1,2,3</id>
    //
    // TestName filter
    //    <test>xxxxxxx.xxx</test>
    //
    // Name filter
    //    <name>xxxxx</name>
    //
    // Namespace filter
    //    <namespace>xxxxx</namespace>
    //
    // Category filter
    //    <cat>cat1</cat>
    //    <cat>cat1,cat2,cat3</cat>
    //
    // Property filter
    //    <prop name="xxxx">value</prop>
    //
    // And Filter
    //    <and><filter>...</filter><filter>...</filter></and>
    //    <filter><filter>...</filter><filter>...</filter></filter>
    //
    // Or Filter
    //    <or><filter>...</filter><filter>...</filter></or>

    public abstract class TestFilterTests
    {
        public const string DUMMY_CLASS = "NUnit.Framework.Tests.Internal.Filters.TestFilterTests+DummyFixture";
        public const string ANOTHER_CLASS = "NUnit.Framework.Tests.Internal.Filters.TestFilterTests+AnotherFixture";
        public const string DUMMY_CLASS_REGEX = "NUnit.*\\+DummyFixture";
        public const string ANOTHER_CLASS_REGEX = "NUnit.*\\+AnotherFixture";

        protected readonly TestSuite DummyFixtureSuite = TestBuilder.MakeFixture(typeof(DummyFixture));
        protected readonly TestSuite AnotherFixtureSuite = TestBuilder.MakeFixture(typeof(AnotherFixture));
        protected readonly TestSuite YetAnotherFixtureSuite = TestBuilder.MakeFixture(typeof(YetAnotherFixture));
        protected readonly TestSuite FixtureWithMultipleTestsSuite = TestBuilder.MakeFixture(typeof(FixtureWithMultipleTests));
        protected readonly TestSuite FixtureWithLongTestCaseNamesSuite = TestBuilder.MakeFixture(typeof(FixtureWithLongTestCaseNames));
        protected readonly TestSuite NestingFixtureSuite = TestBuilder.MakeFixture(typeof(NestingFixture));
        protected readonly TestSuite NestedFixtureSuite = TestBuilder.MakeFixture(typeof(NestingFixture.NestedFixture));
        protected readonly TestSuite EmptyNestedFixtureSuite = TestBuilder.MakeFixture(typeof(NestingFixture.EmptyNestedFixture));
        protected readonly TestSuite TopLevelSuite = new TestSuite("MySuite");
        protected readonly TestSuite ExplicitFixtureSuite = TestBuilder.MakeFixture(typeof(ExplicitFixture));
        protected readonly TestSuite SpecialFixtureSuite = TestBuilder.MakeFixture(typeof(SpecialCharactersFixture));

        [OneTimeSetUp]
        public void SetUpSuite()
        {
            TopLevelSuite.Add(DummyFixtureSuite);
            TopLevelSuite.Add(AnotherFixtureSuite);
            TopLevelSuite.Add(YetAnotherFixtureSuite);
            TopLevelSuite.Add(FixtureWithMultipleTestsSuite);
            TopLevelSuite.Add(NestingFixtureSuite);

            NestingFixtureSuite.Add(NestedFixtureSuite);
            NestingFixtureSuite.Add(EmptyNestedFixtureSuite);
        }

        #region Fixtures Used by Tests

        [Category("Dummy"), Property("Priority", "High"), Author("Charlie Poole")]
        private class DummyFixture
        {
            [Test]
            public void Test()
            {
            }
        }

        [Category("Special,Character-Fixture+!")]
        private class SpecialCharactersFixture
        {
            [Test]
            public void Test()
            {
            }
        }

        [Category("Another"), Property("Priority", "Low"), Author("Fred Smith")]
        private class AnotherFixture
        {
            [Test]
            public void Test()
            {
            }
        }

        private class YetAnotherFixture
        {
        }

        private class FixtureWithMultipleTests
        {
            [Test]
            public void Test1()
            {
            }

            [Test, Category("Dummy")]
            public void Test2()
            {
            }
        }

        private class FixtureWithLongTestCaseNames
        {
            [TestCase("JestlaquissemperlectusMaurisetligulafringillaiaculisnislsagittistemporliberoSedinterdummagnasitametfeugiatullamcorperlectusjustosollicitudinmagnaasollicitudinmagnaaugueveljustoDonecfacilisisinmassanecmollisVivamussitametnullaultriciesaliquammaurisegetgravidarisusDonecleonunccongueeuelementumsedconvallisatortorClassaptenttacitisociosquadlitoratorquentperconubianostraperinceptoshimenaeosSedidipsumnisiEtiameleifendmassavitaetortordauctoraefficixtsapienconvallisVivamushendreritauguesedmattisaliquetDonecideratfacilisiscursussapienauctorrhoncusnislCrasveleratatjustoimperdietmollisEtiamquisorcisitametelitmattisviverraAeneanultriceslacusutdiameuismodutimperdietrisusscelerisqueInhachabitasseplateadictumstAeneandapibusefficiturnequenonullamcorperNuncatmetussagittisinterdumligulaegetultriciesaugueAeneandignissimiaculisvariusAliquaminturpisquisnuncelementumtempusLoremipsumdolorsitametconsecteturadipiscingelitCurabiturornarediamnecelitluctusvenenatisFusceinipsumvelitQuisquedictumultriceseleifendSedquisurnaetarcufaucibusgravidainnonsemVestibulumanteipsumprimisinfaucibusorciluctusetultricesposuerecubiliacurae;SedsagittisplaceratipsumidornareanteAliquamsollicitudinelementumconvallisClassaptenttacitisociosquadlitoratorquentperconubianostraperinceptoshimenaeosMorbisitametsapienpulvinarsagittisquamultriciesposuereturpisDuissollicitudinvariusultriciesCraspharetranislinsollicitudineuismodfelislacuspulvinarorciinullamcorperjustotortorpretiummaurisEtiamacfelisdictumpellentesquemassaeuviverraenimNullamsollicitudinnisisedantemalesuadanecelementumlacushendreritDonecleonuncvolutpatquissuscipitdignissimiaculisnecarcuMaurisconguelacusegetnunccommodoutmalesuadasemullamcorperIntegerliberonibhinterdumvitaenislauctorpellentesqueconguesapienMaurisnonexegetesttinciduntluctusutnectortorCurabitureununcquiseratviverrafermentumidacnislInnonsemarisuspellentesqueultriciesininduiSedmaximusefficiturnibhaiaculisquamposueresedVivamusvolutpatelitnecpuruseuismodvelvolutpatdolorfeugiatNullameuenimetloremaccumsanvenenatisUtvariusimperdietipsumvitaetinciduntipsumscelerisquevehiculaAliquammassalacusblanditvitaediamquisultriciesblanditeratSedacnislcommodogravidaligulaacinterdumleoMorbiultriciessodalespurusacaccumsanlectusfringillaporttitorSedposuerelaciniaporttitorDuisnonloremscelerisquesodaleseratnecvariusmetusVestibulummalesuadanuncvitaeestfaucibusfringillaMaecenaspulvinarrutrummollisCurabitursitameteuismodturpisNametmolestielacusvitaepharetraipsumUtcursusturpiscondimentumcursusegestasNullaquissollicitudinvelitpulvinaregestasduiDuiscondimentumdictumestnoninterdumpurusimperdiethendreritCurabiturornarevelmassaacdapibusAeneaneleifendluctustortorinplaceratdiamfeugiatsitametDonechendreritnonsematbibendumProinvitaeipsumfacilisisvestibulumsemegetmolestiequamNampretiumminecsagittisconguepurusarcuconvalliserosvitaescelerisqueantequamnecloremDonecultricespharetraeuismodCurabituracarcumiMaecenasquistellusnisiInpulvinarelementumiaculisPellentesqueultricesinturpisacmollisNullalobortisnuncestacvehiculamassamolestieegetAeneanlacustellusfermentumvitaeauguevelvariusdapibusjustoInsollicitudinmetusutconsecteturdictumturpisestfermentumdolornonlaciniaquamrisusquisleoAeneanaugueauguetinciduntvitaemagnanonProinvitaeipsumfacilisisvestibulumsemegetmolestiequamNampretiumminecsagittisconguepurusarcuconvalliserosvitaescelerisqueantequamnecloremDonecultricespharetraeuismodCurabituracarcumiMaecenasquistellusnisiInpulvinarelementumiaculisPellentesqueultricesinturpisacmollisNullalobortisnuncestacvehiculamassamolestieegetAeneanlacustellusfermentumvitaeauguevelvariusdapibusjustoInsollicitudinmetusutconsecteturdictumturpisestfermentumdolornonlaciniaquamrisusquisleoAeneanaugueauguetinciduntvitaemagnanonAeneanaugueauguetinciduntvitaemagnanonProinvitaeipsumfacilisisvestibulumsemegetmolestiequamNampretiumminecsagittisconguepurusarcuconvalliserosvitaescelerisqueantequamnecloremDonecultricespharetraeuismodCurabituracarcumiMaecenasquistellusnisiInpulvinarelementumiaculisPellentesqueultricesinturpisacmollisNullalobortisnuncestacvehiculamassamolestieegetAeneanlacustellusfermentumvitaeauguevelvariusdapibusjustoInsollicitudinmetusutconsecteturdictumturpisestfermentumdolornonlaciniaquamrisusquisleoAeneanaugueauguetinciduntvitaemagnanon")]
            [TestCase("己六消朱，麼活斤魚植像邊音母二錯意尤海品四、了背上衣年比杯教乙具還弓快半科木下急向新。言意青您河公；聲成免校尤樹水成至蝶兆入戶成學要正牛比半「詞娘員祖加午男尺」弟才完找或跳牠，福玉弓吉借己。入屋它快元。掃借村第飽鴨很生幼河躲，日事故食國別方用一具嗎枝視京雞還流邊果共？老過飯半助頭訴久她麼以躲七布虎物現葉。消爸一哥這行人苗姊步小見位斥至來石園瓜喜，氣她位耍告以放故喜七吧喝菜姐買字還戊百時。水是出土地馬急南語牛家立就自，想停勿！南話犬象時示告！秋戊主，少丟你。童游者知親去雪羽念還欠平奶視課有米，四隻內課玉念星房黑文白：幫游只借游住員，汗貫乞寫白兌燈工幼方火采口？荷位文兩園生北後；幼習苦泉外。四南彩對瓜誰抱聽直司泉時內口尾封，消比的白尾羽對耳呢有奶就巾裏物邊地視色「實」只因跑右何，士平它，汁生課師具過尺聲子豆飯草支土泉吉肖到！夏太它朋刃音院千。着亮五；父品已念開再、荷汗去央哪？婆文高功來帶姊。哭背知孝魚上光正几心貓；斥圓忍着帶那澡福飽春告不。飯雨唱身化快陽王清久。且教氣連。\r\n心的念知千牠子都兔丟但比，隻欠見室向比跟還且虎隻。圓抱秋明紅幾。元雪清沒刀一黑斥現又畫信邊因重兌父子旁。三跟娘從給往公玉清七帶黑怎。弓示入前士背院以玉九開良眼找皮村寸完，牠樹日邊皮弓點跟未麻鴨人掃水問斗自：林真拍怕一乾身波後買美。他嗎錯巴過尤斗許的爸金反亮、訴門帽方根很夏九；媽扒母但。\r\n以牠圓？蝶雞流肖山多借清即貫誰現室小力吉向。兩爪南告錯彩幾哪穿交爬飛娘園未畫，哭玉孝欠室昌荷它：哥圓沒爪條東蝶，直斗母牠。王造司就忍條了語種登巴好王字快語木知燈「升」空唱尤高筆還立汗每不吧八走可流邊，葉千氣怪？正功追果對耳牠多假陽卜文由、造壯下結棵完蝸步尺口收。內前原具隻波音麻央嗎牠圓亮扒汗午都音開想，根歌用它木澡重登。母長沒植尤幼亭歡玩做少「吹元裏開牙」巴比四未各澡蛋交一苦飽村食！在對天火貝誰吉戊二過貓耳色扒目丁石。秋英來肖大什由斗豆幫太石。開種綠半果往四助禾，玉紅泉花明手加美冬現把苗公跳貝竹以，身晚壯三：蝸雲位飯具女飽。木出兌福太「他豆沒木家化已要不以」躲巾耳直毛英尤誰陽內公小。我交要已色太色經流說、錯音卜几造巴：雨歡功丟像兔色片親「大很」校目筆頁氣葉木道。消早常魚語村來相個月對由您告。平寺四尼連主何科松：買天大河蛋後苗牛再苦實把田就，道姊示哥收完干登實紅文，斥杯虎巴看辛氣穴冒冰身言。尺歡鳥喜裝首邊羊蝶？忍九耍品吹二")]
            public void Test1(string value)
            {
            }
        }

        private class NestingFixture
        {
            public class NestedFixture
            {
                [Test]
                public void Test()
                {
                }
            }

            internal class EmptyNestedFixture
            {
            }
        }

        [Explicit]
        private class ExplicitFixture
        {
        }

        public class CompoundCategoryFilterFixture
        {
            [TestFixture, Category("TestsA")]
            public class TestsA
            {
                [Test]
                public void TestInA()
                {
                }
            }

            [TestFixture, Category("TestsB")]
            public class TestsB
            {
                [Test]
                public void TestInB()
                {
                }
            }

            [TestFixture, Category("TestsC")]
            public class TestsC
            {
                [Test]
                public void TestInC()
                {
                }
            }
        }

        #endregion
    }
}
