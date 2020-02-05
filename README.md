# ApiExample

Exemplo de estrutura e configuração de uma API Rest feita em .Net Core 3.1
Ferramentas utilizadas:

- Entity Framework Core InMemory
- AutoMapper
- Identity
- JWT
- Swagger
- Testes com xUnit

---

## AutoMapper

Utilizado para mapear modelos que são gravados em banco com modelos que são utilizados para serem transportados como dados (DTO).

## Identity

Utilizado configurações básicas e padrões para controle de usuário.

## JWT

Padrão Bearer foi utilizado para autenticação e autorização da API.

## Swagger

Ferramenta para gerar documentação da API a partir das rotas, objetos e respostas.

## Testes com xUnit

xUnit foi utilizado para preparar o projeto de teste. Está sendo testado todas as rotas e métodos importantes para a aplicação.
Os testes são realizado em cima de um banco InMemory populado para haver dados a serem testados.
