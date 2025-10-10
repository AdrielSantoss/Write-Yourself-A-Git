# 📝 Write Yourself A Git

Implementação educacional e didática de comandos básicos do Git em C#.
O objetivo é entender os conceitos internos do Git (blobs, trees, commits, índice, refs)
recriando comandos essenciais e utilitários.

## Estrutura principal
- `Git/` - Implementação dos comandos e do programa principal (`Program.cs`).
  - `Commands/` - Comandos CLI implementados (entrada via `gitadr <comando>`):
    - `init` - Inicializa um repositório (cria `.gitadr`).
    - `hash-object` - Calcula o hash (SHA-1) de um arquivo e opcionalmente grava o blob (`-w`).
    - `cat-file` - Exibe o conteúdo de um objeto (blob/tree/commit) por SHA-1.
    - `write-tree` - Calcula e grava trees a partir do diretório de trabalho.
    - `ls-tree` - Lista o conteúdo de uma tree.
    - `add` - Adiciona arquivos/dirs ao índice (staging).
    - `rm` - Remove arquivos do índice.
    - `restore` - Restaura arquivos do `HEAD` para o workspace ou para o índice (`--staged`).
    - `commit` - Cria um commit com a árvore atual do índice.
    - `log` - Exibe o histórico de commits.
    - `branch` - Cria/lista branches.
    - `switch` - Troca entre branches.
    - `status` - Mostra o estado do workspace, índice e HEAD.
    - `merge` - Faz merge simples entre branches.
    - `diff` - Mostra diferenças entre workspace/index/commits (usa `diff_tool.dll`).
  - `Store/` - Objetos principais:
    - `TreeObject.cs`, `CommitObject.cs`, `ObjectStore.cs` - leitura/gravação de objetos.

- `Git.Core/` - Utilitários e helpers usados pelos comandos:
  - `BlobUtils.cs`, `TreeUtils.cs`, `CommitUtils.cs`, `BranchUtils.cs`, `IndexUtils.cs`, `Sha1Utils.cs`, etc.

- `Git.Test/` - Testes unitários dos comandos.

## Como executar

1. Abra o projeto no .NET (solution `Write.Yourself.A.Git.sln`).
2. (Opcional) Rode o script de instalação PowerShell em `Installer/install.ps1` para registrar/empacotar o binário.
3. A aplicação principal é o executável que expõe o comando `gitadr`. Você pode executar localmente com dotnet:

```powershell
dotnet run --project .\Git\Git.csproj -- <comando> [args]
```

Exemplo: `dotnet run --project .\Git\Git.csproj -- init`.

Depois de construir/instalar, o binário pode ser chamado como `gitadr <comando>` conforme o `Program.cs`.

## Comandos (resumo e exemplos)

- init
  - Uso: `gitadr init`
  - Cria diretório `.gitadr` com estrutura mínima.

- hash-object
  - Uso: `gitadr hash-object [-w] <arquivo>`
  - Sem `-w` retorna o SHA-1; com `-w` grava o blob no objeto store.

- cat-file
  - Uso: `gitadr cat-file <sha1>`
  - Mostra o conteúdo do objeto (blob/tree/commit).

- write-tree
  - Uso: `gitadr write-tree`
  - Percorre o diretório de trabalho montando trees e gravando-as, retorna o SHA-1 da tree raiz.

- ls-tree
  - Uso: `gitadr ls-tree <tree-sha1>`
  - Lista entradas de uma tree (modo, sha1, nome).

- add
  - Uso: `gitadr add <arquivo|diretório|.>`
  - Adiciona ao índice. `.` adiciona todo o workspace.

- rm
  - Uso: `gitadr rm <arquivo>`
  - Remove arquivo do índice (staging).

- restore
  - Uso: `gitadr restore [--staged] <arquivo|diretório|.>`
  - Restaura conteúdo do último commit (`HEAD`) para o workspace ou apenas para o índice com `--staged`.

- commit
  - Uso: `gitadr commit -m "mensagem"` (a implementação trata criação de commit usando árvore do índice)

- log
  - Uso: `gitadr log`
  - Exibe histórico de commits a partir do `HEAD`.

- branch
  - Uso: `gitadr branch <nome>`
  - Cria (ou lista) branches.

- switch
  - Uso: `gitadr switch <branch>`
  - Troca o `HEAD` para outro branch e ajusta workspace conforme necessário.

- status
  - Uso: `gitadr status`
  - Mostra: staged, modified (não staged), deleted, untracked.

- merge
  - Uso: `gitadr merge <branch>`
  - Efetua merge simples entre branches (conflitos podem ser mostrados na saída).

- diff
  - Uso: `gitadr diff [--patience]` ou `gitadr diff --staged` ou `gitadr diff HEAD` ou `gitadr diff <commit1> <commit2>`
  - Mostra diferenças usando `diff_tool.dll` (Myers/Patience algorithm).

## Implementações e utilitários

- `IndexUtils` - leitura, escrita e normalização do índice (`.gitadr/index`), leitura recursiva do workspace.
- `BlobUtils` / `Sha1Utils` - gravação de blobs, leitura de dados por SHA-1, escrita de arquivos a partir de um objeto.
- `TreeUtils` / `CommitUtils` - montagem/parse de trees e commits.
- `BranchUtils` - manipulação de refs e HEAD.
- `ObjectStore` - grava e lê objetos no diretório `.gitadr/objects`.

## Testes

O projeto contém testes em `Git.Test/`. Para executar os testes (assumindo .NET SDK instalado):

```powershell
dotnet test .\Git.Test\Git.Test.csproj
```

## Notas e limitações

- Projeto educacional: não cobre todos os casos de uso do Git (segurança, performance, formatos complexos de objetos, hooks, rede, etc.).
- Algumas operações usam convenções simplificadas (por exemplo, formato do arquivo de índice, tratamento de modos de arquivo).
- `diff` depende de `diff_tool.dll` presente em `Git/bin/Debug/net9.0` (ou `Libs/diff_tool.dll`) para cálculo de diferenças.

---
Adriel