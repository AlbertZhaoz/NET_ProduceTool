using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AlbertZhao.cn.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }


        [HttpPost]
        public ActionResult<LoginResponse> LoginByUserName(LoginRequest request)
        {
            if (request.UserName == "admin" && request.Password == "123")
            {
                var items = Process.GetProcesses().Select(p => new ProcessInfo(p.Id, p.ProcessName, p.WorkingSet64));
                return new LoginResponse(true, items.ToArray());
            }
            else
            {
                return new LoginResponse(false, null);
            }
        }


        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        public ActionResult<string> PrintStudent(int id)
        {
            if(id == 1)
            {
                var person = new Person("Albert", "Zhao");
                return person.PrintFullNameById(1);
            }
            else
            {
                return NotFound("��Ա������");
            }
        }

        //�������ݵĺô����ӷ���Restful���
        [HttpGet("{classNum}/{id}")]
        public ActionResult<string> PrintInfoByClassNumAndId(int classNum,int id)
        {
            return new Person("albert", "zhao").PrintInfo(classNum,id);
        }

        //�������ݵĺô����ӷ���Restful���
        [HttpGet("{schoolNum}/{id}")]
        //�����׽��ֵ�ͺ����������ֲ�һ�£�������[FromRoute(Name="����")]��ָ��
        public ActionResult<string> PrintInfoBySchoolNoAndId([FromRoute(Name = "schoolNum")]int schoolNo, int id)
        {
            return new Person("albert", "zhao").PrintInfo(schoolNo, id);
        }

        [HttpPost]
        public async Task<ActionResult<string>> AddPerson(Person person)
        {
            try
            {
                await System.IO.File.WriteAllTextAsync(@"F:\Test\test.txt", person.ToString());
                return "Add person successfully!";
            }
            catch (Exception)
            {
                return NotFound($"Failed!{person.ToString()}");
            }
        }

        [HttpPut("{id}")]
        public string UpdatePerson([FromRoute(Name ="id")]int id,Person p,[FromHeader(Name ="User-Agent")] string us)
        {
            return $"���³ɹ�:{us}";
        }

        [HttpGet]
        public string LoveChen()
        {
            return "�С��\nС����ɰ�\nôô��\n";
        }
    }

    public record Person(string firstName, string lastName)
    {
        public string PrintFullNameById(int id)
        {
            return firstName + " " + lastName; 
        }

        public string PrintInfo(int classNum,int id)
        {
            return $"ѧ������Ϊ{firstName+lastName}\n" +
                $"ѧ���༶Ϊ:{classNum}\n" +
                $"ѧ��idΪ{id}\n";
        }
    }

    public record LoginRequest(string UserName,string Password);
    public record ProcessInfo(long ID,string Name,long WorkingSet);
    public record LoginResponse(bool OK, ProcessInfo[] ProcessInfos);
}