# cmstar.WebApi

[![NuGet](https://img.shields.io/nuget/v/cmstar.WebApi.svg)](https://www.nuget.org/packages/cmstar.WebApi/)

一个极速的WebAPI开发库，能够以简单且非侵入的方式将任何 .net 方法发布为 WebAPI 。

支持的 .NET 版本：
- .NET Framework 4.6 或更高版本。
- .NET Core 2.1 或更高版本。
- 其他支持 .NET Standard 2 的运行时。

## 安装

通过 Package Manager:
```
Install-Package cmstar.WebApi
```

或通过 dotnet-cli:
```
dotnet add package cmstar.WebApi
```

## 目标

- 将代码与通信协议解耦，业务方法上不会体现 URL 相关的信息。
- 傻瓜化。可牺牲通用性，换取便捷。

## 设计思路

让 WebAPI 和 .net 方法一一映射。

一个 WebAPI 与代码的对应关系可抽象为下述内容：
- API 入口，对应一个 .net 方法。
- 输入，对应 .net 方法的输入参数。
- 输出，对应 .net 方法的结果：
  - 若方法成功执行，则结果就是方法的返回值（或者 void ）；
  - 若方法执行失败，则抛出一个异常。

当前库使用一组固定的方式描述这几方面的内容。 
1. 方法的入口，体现在 URL 上，对应 `~method` 元参数。
2. 输出一个固定的格式，由 `Code` 字段表示是否有错误；`Data`表示执行成功时的返回值；`Message`给出异常时的信息。详见下文《返回的格式》一节。

## 入门——三步构建一个WebAPI

假设已有业务逻辑：

```csharp
public class SimpleServiceProvider
{
    private readonly Random _random = new Random();

    public int PlusRandom(int x, int y)
    {
        return x + y + _random.Next(1000);
    }

    // 支持异步方法。
    public static async Task<bool> Save(string value)
    {
        return true;
    }
}
```

现在让我们按下面的步骤，将上面的 `PlusRandom` 和 `Save` 方法发布为WebAPI。

1. 新建一个ASP.net项目，并引用此API程序集。
2. 写一个 API 入口类，继承抽象类`SlimApiHttpHandler`。重写`Setup`方法，代码如下（更详细的使用见下文《注册API方法》节）：
```csharp
public class SimpleExample : SlimApiHttpHandler
{
    public override void Setup(ApiSetup setup)
    {
        setup.Auto(new DemoService(), parseAttribute: false);
    }
}
```
3. 注册 URL ，这里以 .net Framework 版为例：
```csharp
using cmstar.WebApi.Routing;

public class Global : System.Web.HttpApplication
{
    protected void Application_Start(object sender, System.EventArgs e)
    {
        // 使用 CreateApiRouteAdapter 扩展方法创建一个 IApiRouteAdapter ，
        // 它统一了 .net Framework 和 .net Core 的路由注册方式。
        IApiRouteAdapter routes = System.Web.Routing.RouteTable.Routes.CreateApiRouteAdapter();

        // 注册。
        routes.MapApiRoute<SimpleExample>("demo/{~method}");
    }
}
```

如果是 .net Core 版，借助`IApiRouteAdapter`，路由注册部分和 .net Framework 几乎一样：
```csharp
using cmstar.WebApi.Routing;

public class Startup
{
    public void Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder app)
    {
        // 使用 CreateApiRouteAdapter 扩展方法创建一个 IApiRouteAdapter 。
        var routes = app.CreateApiRouteAdapter();

        // 注册。
        routes.MapApiRoute<SimpleExample>("demo/{~method}");
    }
}
```

现在WebAPI已经可以使用了，编译运行，并使用下面的链接访问（不同电脑上端口可能不一样）：

    http://localhost:8042/demo/PlusRandom?x=13&y=25

得到了返回结果，因为方法中使用随机数，返回结果的`Data`字段可能不尽相同：
```json
{"Code":0,"Message":"","Data":311}
```

类似的，访问 `http://localhost:8042/demo/save` ，得到：
```json
{"Code":0,"Message":"","Data":true}
```


## 通信协议

SlimAPI 是一个基于 HTTP WebAPI 的通信契约。

上面的简单示例中可以看出，使用了GET方式访问WebAPI，返回结果为JSON。

### 请求的 URL
可以通过 HTTP 的 Content-Type 头指定使用何种格式请求，目前支持的类型如下：

- GET 不读取 Content-Type 头。
- POST FORM 表单格式， Content-Type 可以是 application/x-www-form-urlencoded 或 multipart/form-data 。
- POST JSON 以 JSON 作为数据，值为 application/json 。

也可以不指定 Content-Type 头，而通过`~format`参数指定格式，详见下文到 URL 形式。

#### URL 形式1

```
http://domain/ApiEntry?~method=METHOD&~format=FORMAT&~callback=CALLBACK
```

以“~”标记的参数为 API 框架的元参数：
- ~method：必填；表示被调用的方法的名称。
- ~format：可选；请求所使用的数据格式，支持get/post/json；此参数在可以在不方面指定`Content-Type`时提供相同的功能。
- ~callback：可选；JSONP回调函数的名称，一旦制定此参数，返回一个 JSONP 结果，Content-Type: text/javascript 。
参数名称都是大小写不敏感的。`~format`参数优先级高于`Content-Type`，若指定了`~format`，则`Content-Type`的值被忽略。

`~format`的可选值：
- get 默认值。使用 GET 方式处理。
- post 效果等同于给定 Content-Type: application/x-www-form-urlencoded
- json 效果等同于给定 Content-Type: application/json

#### URL 形式2

不需要再写参数名字，直接将需要的元参数值追加在URL后面。
```
http://domain/ApiEntry?METHOD.FORMAT(CALLBACK) 
```

同形式1，“.FORMAT”和“(CALLBACK)”是可选的， 省略“.FORMAT”后形如： `http://domain/ApiEntry?METHOD(CALLBACK)` ； 
省略“(CALLBACK)”后形如： `http://domain/ApiEntry?METHOD.FORMAT` 。

#### URL 形式3

通过路由规则，将元参数编排到 URL 路径里。
这是最常见的方案： `http://domain/ApiEntry/METHOD` 这里 METHOD 就是元参数 ~method 。
前面的例子中使用的就是这种方式。


### 请求的参数

请求参数 
- GET 参数体现在 URL 上，形如 data=1&name=abc&time=2014-4-8 。 
- 表单 以 POST 方式放在HTTP BODY中。 
- JSON 只能使用 POST 方式上送。

可在 GET/表单参数中传递简单的数组，数组元素间使用 ~ 分割，如 1~2~3~4~5 可表示数组 [1, 2, 3, 4, 5]。

在GET/表单格式下：
```
data=1&name=abc&time=2014-4-8&array=1~2~3~4
```

与JSON格式下的下面内容等价：
```json
{ "data":1, "name": "abc", "time": "2014-4-8", "array": [1, 2, 3, 4] }
```

日期格式使用字符串的 yyyy-MM-dd HH:mm:ss 格式，默认为 UTC 时间。也支持 RFC3339 ，这种格式自带时区。

### 返回的格式

若指定了 ~callback 参数，则返回结果为 JSONP 格式： `Content-Type: text/javascript` ；
否则为 JSON 格式： `Content-Type: application/json` 。

状态码总是200，具体异常码需要从Code字段判定。数据装在一个基本的信封中，信封格式如下：
```json
{ Code: 0, Message: "", Data: {} }
```

- Code 0为API调用成功未见异常，非0值为异常：
    - 1-999 API请求、通信及运行时异常，尽可能与 HTTP 状态码一致：
        - 500 服务端内部异常。
        - 400 请求参数或报文错误。
        - 403 客户端无访问权限。
    - 其他约定：
    - -1 未明确定义的错误。
    - 1000-9999 预留
    - 大于等于10000为业务预定义异常，由具体API自行定义，但建议至少分为用户可见和不可见两个区间：
        - 10000-19999 建议保留为不提示给用户的业务异常，对接 API 的客户端对于这些异常码，对用户提示一个统一的如“网络异常”的错误。
        - 20000-29999 对接 API 的客户端可直接将 Message 展示给用户，用于展示的错误消息需要由服务端控制的场景。
- Message 附加信息， Code 不为0时记录错误描述，可为空字符串。
- Data 返回的主数据，不同API各不相同，其所有可能形式如下：
    - 若API没有返回值，则为 null 。
    - 对于返回布尔型结果的 API ，Data 为 true 或 false 。
    - 对于返回数值结果的 API ，Data即为数值，如：123.654 。
    - 对于返回字符串结果的 API ， Data 为字符串，如："string value" 。
    - 日期也作为字符串，参照前面提到的日期格式。
    - 对于返回集合的 API ， Data 为数组，如： ["result1", "result2"] 。
    - 对于返回复杂对象的 API ，Data 为 JSON object ，如：{ "Field1": "Value1", "Field2": "Value2" } 。


## 注册API方法

`SlimApiHttpHandler.Setup`方法提供了API注册的入口。

前面的例子中给出了API方法注册的入口

```csharp
public class SimpleExample : SlimApiHttpHandler
{
    public override void Setup(ApiSetup setup)
    {
        // 在这里通过setup实例进行方法注册
    }
}
```

### 通过委托注册与方法的重命名

可以通过委托，注册当个方法：

```csharp
setup.Method((Func<int, int, int>)serviceProvider.PlusRandom);
```

对应调用时的`~method`即为方法名称，其实也可以对调用名称进行重命名：

```csharp
setup.Method((Func<int, int, int>)serviceProvider.PlusRandom).Name("plus");
```

则对应的调用`PlusRandom`方法的`~method`变为了 plus 。

利用委托机制，可以注册匿名方法：

```csharp
Func<int, int, int> multiple = (x, y) => x * y;
setup.Method(multiple).Name("multiple");
```

### 通过`MethodInfo`注册

可以直接注册`MethodInfo`：

```csharp
// 静态方法
var method = typeof(SimpleServiceProvider).GetMethod("Save");
setup.Method(null, method); 

// 实例方法
method = typeof(SimpleServiceProvider).GetMethod("PlusRandom");
setup.Method(new SimpleServiceProvider(), method);
```

*如果你想，明显可以注册私有方法。*

### 批量注册

可以利用`ApiMethodAttribute`特性标记方法

```csharp
public class SimpleServiceProvider
{
    [ApiMethod]
    public int Plus(int x, int y) { return x + y; }

    [ApiMethod]
    public void Save(string value) {  }

    // 没有ApiMethod不会被注册
    public void WillNotBeRegistered() {  }
}
```

之后通过使用`Auto`方法注册所有有特性的方法：

```csharp
setup.Auto(new SimpleServiceProvider());
```

`Auto`方法要求提供包含待注册方法的实例，对于静态类，则使用`FromType`方法，如：

```csharp
setup.FromType(typeof(SomeStaticClass));
```

如果不想使用`ApiMethodAttribute`，可以通过重载方法的参数指定需要注册的方法的范围，
例如下面的例子注册类型内所有非私有的静态方法：

```csharp
setup.Auto(new SimpleServiceProvider(),
    parseAttribute: false, 
    bindingFlags: BindingFlags.Static | BindingFlags.Public);
```

最开始的示例中，使用的就是这种方式，并且使用默认的 `bindingFlags` 参数值——注册所有公开的方法。

### 其他需要注意的

- 若通过对象实例进行注册，则实例将被保存下来，该实例不会被垃圾回收。


## 作为 WebAPI 的方法

### 接收复杂对象

上文示例中，`PlusRandom` 方法的 `x/y` 参数是直接方法的参数表上的。随着需求的复杂化，参数逐渐增多，就应当将参数重新定义到一个类里去。

在.net方法只有一个 [POCO](https://en.wikipedia.org/wiki/Plain_old_CLR_object) 参数，且参数类型中的所有属性和字段都能够从HTTP参数转换，
此时，使用  JSON 传参，HTT P参数名称和 JSON 属性名称将直接和该 POCO 参数的属性和字段名称进行匹配。

前面的`PlusRandom`方法也可以被改成下面样子，而调用方的请求则不需要改变：

```csharp
public class PlusRandomParam
{
    public int x { get; set; }  // 可以用属性。
    public int y；              // 也可以省事直接用字段。
}

public int PlusRandom(PlusRandomParam param)
{
    // 实现逻辑
}
```

### 处理文件流

若方法参数使用两个特定的类型之一`System.IO.Stream`及`System.Web.HttpFileCollection`作为输入参数，则该参数的值将为该次请求的：

- `Stream` 类型的参数将获得 `HttpContext.Current.Request.InputStream`；
- `HttpFileCollection` 类型的参数将获得 `HttpContext.Current.Request.Files` ；

比如可以用下面这个方法处理HTTP文件上传，并且在URL中接收id和name两个参数，方法最终返回被上传文件的URL地址

```csharp
[ApiMethod]
public void Upload(int id, string name, HttpFileCollection files)
{
    for (int i = 0; i < files.Count; i++)
    {
        var file = files[i];
        // process the file
    }
}
```


## 调用者/客户端

通信协议固定后，就可以封装调用过程了，一个 WebAPI 由下面要素构成：
- 入口。
- 方法名称。
- 输入，即参数表。
- 输出，包括正常输出和错误输出。

现成的调用类是 `cmstar.WebApi.Slim.SlimApiInvoker`：
```csharp
var res = SlimApiInvoker.Invoke<int>(                  // 输出通过泛型表示。
    "http://localhost:8042/demo/PlusRandom/{~method}", // 入口。
    "PlusRandom",                                      // 方法名称。
    new { x = 13, y = 25 });                           // 输入。

Console.WriteLine(res); // -> 311
```

也可以使用实例方法：
```csharp
// 可以将实例保存起来方便复用。
var invoker = new SlimApiInvoker("http://localhost:8042/demo/PlusRandom/{~method}");

// 调用。
var res = invoker.Invoke<int>("PlusRandom", new { x = 13, y = 25 });
```

错误处理：
1. 若被调用的 WebAPI 返回 `Code` 不是0， `SlimApiInvoker` 会抛出一个带有 `Code` 和 `Message` 的 `ApiException` 。
2. 可以使用 `RawInvoke` 方法替代 `Invoke` 方法，它会返回带有  `Code` 和 `Message` 的对象 `ApiResponse` 。


## 其他语言的版本

- GO 版： [cmstar/go-webapi](https://github.com/cmstar/go-webapi)