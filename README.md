The Ncqrs Framework
===================

Ncqrs is a framework for .NET helps build scalable, extensible and maintainable
applications by supporting developers apply the Command Query Responsibility
Segregation (CQRS) architectural pattern. It does so by providing an
infrastructure and implementations for the most important building blocks for
command handling, domain modeling, event sourcing, and so. These building blocks
help you to focus on the code that adds business value. They come with
annotation, convention and configuration support and help you to write isolated
and testable.


Build locally
-------------
Run `BUILD.bat` to build the Ncqrs Framework. This will build the framework, run
all the tests and updates all the lib folders of other solutions (extensions and
sample) with the result.


Discussion and feedback
-----------------------
The best source for discussion is the [Ncqrs-dev group][1]. You could
also drop quick messages to us using [twitter][2].

To submit a bug or make a feature request use our [Redmine environment][3].

[1]: http://groups.google.com/group/ncqrs-dev "Ncqrs-dev group"
[2]: http://twitter.com/ncqrs/ "@Ncqrs at twitter"
[3]: http://redmine.ncqrs.org/ "Ncqrs redmine environment"

Contribution
------------
Since the code base of Ncqrs will never be complete. We encourage users to 
[fork][4] Ncqrs code, make changes, commit them to your forked repository, and 
submit [pull requests][5].

You can use our [Redmine environment] to find support our project management. Use it to
submit a bug, request a feature and to see our roadmap.

[3]: http://redmine.ncqrs.org/ "Ncqrs redmine environment"
[4]: http://help.github.com/forking/ "Fork guide"
[5]: http://github.com/guides/pull-requests "Pull request guide"

Why does git show that all of my files are modified?
----------------------------------------------------	
Ncqrs is built by Windows users, so all of the text files have CRLF line 
endings. These line endings are stored as-is in git (which means we all have 
autocrlf turned off).
If you have autocrlf enabled, when you retrieve files from git, it will modify
all of your files. Your best bet is to turn off autocrlf, and re-create your
clone of Ncqrs.

1. Delete your local clone of the Ncqrs repository
1. Type: `git config --global core.autocrlf false`
1. Type: `git config --system core.autocrlf false`
1. Clone the Ncqrs repository again

License
-------
The Ncqrs framework and its documentation are licensed under the Apache License,
Version 2.0 (the "License"); you may not use this file except in compliance with
the License. You may obtain a copy of the License at 
<http://www.apache.org/licenses/LICENSE-2.0>.

Unless required by applicable law or agreed to in writing, software distributed 
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
CONDITIONS OF ANY KIND, either express or implied. See the License for the 
specific language governing permissions and limitations under the License.

[![Bitdeli Badge](https://d2weczhvl823v0.cloudfront.net/ncqrs/ncqrs/trend.png)](https://bitdeli.com/free "Bitdeli Badge")

