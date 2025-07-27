# 📝 Write Yourself A Git

Este projeto é uma implementação educacional de comandos básicos do Git em C#. O objetivo é aprender como o Git funciona internamente, recriando funcionalidades essenciais de forma didática.

**Status:** Em desenvolvimento 🚧

## 📁 Estrutura do Projeto
- `Git/` - Implementação dos comandos principais do Git.
  - `Commands/` - Comandos implementados:
    - `CatFile.cs` - Exibe o conteúdo de objetos armazenados.
    - `HashObject.cs` - Calcula o hash SHA-1 de arquivos e armazena como objeto.
    - `Init.cs` - Inicializa um novo repositório Git.
  - `Core/` - Lógica interna para manipulação de objetos Git:
    - `ObjectStore.cs` - Gerenciamento de armazenamento de objetos.
    - `TreeObject.cs` - Manipulação de árvores de objetos.
- `Git.Core/` - Utilitários e funções de apoio para o funcionamento dos comandos.
- `Git.Test/` - Testes automatizados para os comandos implementados.
  - `CatFileTest.cs`, `HashObjectTest.cs` - Testes para os comandos já prontos.

## ▶️ Como executar
1. Abra a solução `Write.Yourself.A.Git.sln` no Visual Studio.
2. Compile o projeto.
3. Execute os comandos disponíveis pelo terminal ou configure para rodar pelo Visual Studio.

## ✅ Funcionalidades já implementadas
- Inicialização de repositório (`init`)
- Exibição de objetos (`cat-file`)
- Hash e armazenamento de arquivos (`hash-object`)

## 🛠️ Funcionalidades planejadas
- Implementação de outros comandos do Git
- Melhorias na interface de linha de comando
- Documentação detalhada

## ⚠️ Aviso
Este projeto está em fase inicial de desenvolvimento. Muitas funcionalidades do Git ainda não foram implementadas e podem haver mudanças frequentes na estrutura do código.

## 📚 Referências
- [Write Yourself a Git](https://wyag.thb.lt/) (tutorial base)
- Documentação oficial do Git

---
Adriel Santos
