# 📝 Write Yourself A Git

Este projeto é uma implementação educacional de comandos básicos do Git em C#. O objetivo é aprender como o Git funciona internamente, recriando funcionalidades essenciais de forma didática.

**Status:** Em desenvolvimento 🚧

## 📁 Estrutura do Projeto
- `Git/` - Implementação dos comandos principais do Git.
  - `Commands/` - Comandos implementados:
    - `Init.cs` - Inicializa um novo repositório Git.
    - `HashObject.cs` - Calcula o hash SHA-1 de arquivos e armazena como blobs.
    - `CatFile.cs` - Exibe o conteúdo de objetos armazenados.
    - `WriteTree.cs` - Calcula o hash SHA-1 de arquivos e diretórios, armazena como blobs e trees.
    - `Add.cs` - Adiciona arquivos ao índice.
    - `Commit.cs` - Cria um commit com os objetos atuais.
    - `LsTree.cs` - Lista o conteúdo de uma tree.
    - `Rm.cs` - Remove arquivos do indice.
    - `Log.cs` - Exibe o histórico de commits.
    - `Branch.cs` - Cria novos branches.
    - `Switch.cs` - Troca entre branches.
    - `Status.cs` - Mostra o estado da árvore de trabalho e staging.
    - `Merge.cs` - Faz merge entre branches.
  - `Core/` - Lógica interna para manipulação de objetos Git:
    - `ObjectStore.cs` - Gerenciamento de armazenamento de objetos.
    - `TreeObject.cs` - Manipulação de árvores de objetos.
    - `CommitObject.cs` - Manipulação de objetos commit.
- `Git.Core/` - Utilitários e funções de apoio para o funcionamento dos comandos.
- `Git.Test/` - Testes automatizados para os comandos implementados.

## ▶️ Como executar
1. Clone o projeto
2. Execute o script de instalação em /Installer/installer.ps1.
3. Após instalação, os comandos já estarão disponiveis para serem executados pelo executável "gitadr"

## ✅ Funcionalidades já implementadas
- Inicialização de repositório (`gitadr init`)
- Hash e armazenamento de arquivos (`gitadr hash-object`)
- Exibição de objetos (`gitadr cat-file`)
- Hash e armazenamento de arquivos e diretórios (`gitadr write-tree`)
- Adição ao índice (`gitadr add`)
- Remoção do índice (`gitadr rm`)
- Commit (`gitadr commit`)
- Listagem de trees (`gitadr ls-tree`)
- Log de commits (`gitadr log`)
- Criação de branches (`gitadr branch`)
- Troca de branch (`gitadr switch`)
- Status da árvore de trabalho (`gitadr status`)
- Merge de branches (`gitadr merge`)

## 🛠️ Funcionalidades planejadas
- Implementação de outros comandos do Git
- Melhorias na interface de linha de comando
- Documentação detalhada

## 📚 Referências
- [Write Yourself a Git](https://wyag.thb.lt/)
- Documentação oficial do Git

---
Adriel