# Third-Party Notices

Manager Server includes and redistributes the third-party components listed below.
Each remains the property of its respective copyright holders and is used under the
license shown. Nothing in this file modifies the terms of those licenses, and nothing
in the Manager Server license (see `LICENSE.md`) applies to these components.

Where a license is reproduced in full it appears in the [License texts](#license-texts)
section at the end of this file. Where a license is referenced by URL, a copy is
available at that address.

## Front-end components

Bundled under `wwwroot/resources/` and embedded into the Manager Server executable.

| Component | Version | Copyright | License |
|---|---|---|---|
| [Ace](https://ace.c9.io) | 1.43.3 | Ajax.org B.V. | [BSD-3-Clause](#bsd-3-clause--ace) |
| [Bootstrap](https://getbootstrap.com) | 5.0.1 | 2011–2021 The Bootstrap Authors; Twitter, Inc. | [MIT](#mit) |
| [decimal.js-light](https://github.com/MikeMcl/decimal.js-light) | 2.4.1 | Michael Mclaughlin | [MIT](#mit) |
| [Font Awesome Free](https://fontawesome.com) | 7.3.1 | Fonticons, Inc. | Icons: [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/) · Fonts: [SIL OFL 1.1](https://scripts.sil.org/OFL) · Code: [MIT](#mit) |
| [htmx](https://htmx.org) | 2.0.2 | Big Sky Software | [BSD-2-Clause](#bsd-2-clause--htmx) |
| [jQuery](https://jquery.com) | 1.8.2 | jQuery Foundation and other contributors | [MIT](#mit) |
| [LiquidJS](https://liquidjs.com) | 10.24.0 | Harttle | [MIT](#mit) |
| [mark.js](https://markjs.io) | 8.11.1 | 2014–2018 Julian Kühnel | [MIT](#mit) |
| [math-expression-evaluator](https://github.com/bugwheels94/math-expression-evaluator) | — | Ankur Redkar and contributors | [MIT](#mit) |
| [Papa Parse](https://www.papaparse.com) | 5.3.1 | Matthew Holt | [MIT](#mit) |
| [QRCode.js](https://github.com/davidshimjs/qrcodejs) | — | Sangmin Shim (davidshimjs) | [MIT](#mit) |
| [Select2](https://github.com/select2/select2) | 3.5.4 | 2014 Igor Vaynberg | Dual [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0) or GPL-2.0 — used here under **Apache-2.0** |
| [Sortable](https://github.com/SortableJS/Sortable) | 1.8.4 | All contributors to Sortable | [MIT](#mit) |
| [Tailwind CSS](https://tailwindcss.com) | 4.1.13 | Tailwind Labs, Inc. | [MIT](#mit) |
| [UAParser.js](https://github.com/faisalman/ua-parser-js) | 0.7.18 | 2012–2016 Faisal Salman | Dual GPL-2.0 or MIT — used here under **[MIT](#mit)** |
| [Vue.js](https://vuejs.org) | 2.6.12 | 2014–2020 Evan You | [MIT](#mit) |
| [vue2-datepicker](https://github.com/mengxiong10/vue2-datepicker) | — | mengxiong10 | [MIT](#mit) |
| [vue-select](https://vue-select.org) | — | Jeff Sagal and contributors | [MIT](#mit) |
| [vuedraggable](https://github.com/SortableJS/Vue.Draggable) | — | David Desmaisons | [MIT](#mit) |
| [written-number](https://github.com/yamadapc/js-written-number) | — | Pedro Tacla Yamada | [MIT](#mit) |

Files under `wwwroot/resources/custom/`, `wwwroot/resources/htmx-extensions/`,
`wwwroot/resources/select2vue/` and `wwwroot/resources/themes/` are part of Manager
Server itself and are covered by `LICENSE.md`.

## .NET packages

Restored from NuGet at build time and redistributed inside self-contained builds.

| Package | Version | License |
|---|---|---|
| [AWSSDK.DynamoDBv2](https://github.com/aws/aws-sdk-net) | 4.0.100.2 | [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0) |
| [AWSSDK.S3](https://github.com/aws/aws-sdk-net) | 4.0.100.2 | [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0) |
| [BCrypt.Net-Next](https://github.com/BcryptNet/bcrypt.net) | 4.2.0 | [MIT](#mit) |
| [CsvHelper](https://joshclose.github.io/CsvHelper/) | 33.1.0 | Dual [MS-PL](https://opensource.org/license/ms-pl-html) or [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0) — used here under **Apache-2.0** |
| [FastMember](https://github.com/mgravell/fast-member) | 1.5.0 | [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0) |
| [GoogleAuthenticator](https://github.com/BrandonPotter/GoogleAuthenticator) | 3.2.0 | [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0) |
| [MailKit](https://github.com/jstedfast/MailKit) | 4.17.0 | [MIT](#mit) |
| [Markdig](https://github.com/xoofx/markdig) | 1.3.2 | [BSD-2-Clause](#bsd-2-clause--markdig) |
| [Microsoft.AspNetCore.OpenApi](https://github.com/dotnet/aspnetcore) | 10.0.9 | [MIT](#mit) |
| [Microsoft.AspNetCore.WebUtilities](https://github.com/dotnet/aspnetcore) | 10.0.9 | [MIT](#mit) |
| [Microsoft.Extensions.FileProviders.Embedded](https://github.com/dotnet/aspnetcore) | 10.0.9 | [MIT](#mit) |
| [Newtonsoft.Json](https://www.newtonsoft.com/json) | 13.0.4 | [MIT](#mit) |
| [protobuf-net](https://github.com/protobuf-net/protobuf-net) | 3.2.56 | [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0) |
| [PuppeteerSharp](https://github.com/hardkoded/puppeteer-sharp) | 25.3.1 | [MIT](#mit) |
| [Scalar.AspNetCore](https://github.com/scalar/scalar) | 2.16.10 | [MIT](#mit) |
| [Sentry.AspNetCore](https://github.com/getsentry/sentry-dotnet) | 6.6.0 | [MIT](#mit) |
| [SharpMt940Lib.Core](https://github.com/pmetselaar/SharpMt940Lib) | 1.0.2 | [MIT](#mit) |
| [SQLitePCLRaw.bundle_e_sqlite3](https://github.com/ericsink/SQLitePCL.raw) | 3.0.3 | [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0) |

`SQLitePCLRaw.bundle_e_sqlite3` embeds [SQLite](https://www.sqlite.org), which is in
the public domain.

## Runtime

Self-contained builds include the [.NET runtime](https://github.com/dotnet/runtime),
© .NET Foundation and Contributors, licensed under [MIT](#mit).

PDF generation downloads and runs [Chromium](https://www.chromium.org) via
PuppeteerSharp. Chromium is not redistributed with Manager Server; it is fetched at
runtime and licensed under [BSD-3-Clause and other licenses](https://chromium.googlesource.com/chromium/src/+/main/LICENSE).

## License texts

### MIT

```
Permission is hereby granted, free of charge, to any person obtaining a copy of this
software and associated documentation files (the "Software"), to deal in the Software
without restriction, including without limitation the rights to use, copy, modify,
merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be included in all copies
or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
```

### BSD-3-Clause — Ace

```
Copyright (c) 2010, Ajax.org B.V.
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are
permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this
      list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this
      list of conditions and the following disclaimer in the documentation and/or
      other materials provided with the distribution.
    * Neither the name of Ajax.org B.V. nor the names of its contributors may be used
      to endorse or promote products derived from this software without specific
      prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
DAMAGE.
```

### BSD-2-Clause — htmx

```
Copyright (c) 2020, Big Sky Software
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are
permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list
   of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this
   list of conditions and the following disclaimer in the documentation and/or other
   materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
DAMAGE.
```

### BSD-2-Clause — Markdig

```
Copyright (c) 2018-2019, Alexandre Mutel
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are
permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list
   of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this
   list of conditions and the following disclaimer in the documentation and/or other
   materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
DAMAGE.
```
