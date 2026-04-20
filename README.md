[![Deploy](https://github.com/joaosena19/fiap-12soat-projeto-fase-5-processamento/actions/workflows/deploy.yaml/badge.svg)](https://github.com/joaosena19/fiap-12soat-projeto-fase-5-processamento/actions/workflows/deploy.yaml)

# Identificação

Aluno: João Pedro Sena Dainese  
Registro FIAP: RM365182  

Turma 12SOAT - Software Architecture  
Grupo individual  
Grupo 93  

Discord: joaodainese  
Email: joaosenadainese@gmail.com  

## Sobre este Repositório

Este repositório contém apenas parte do projeto completo da Fase 5. Para visualizar a documentação completa, diagramas de arquitetura, e todos os componentes do projeto, acesse: [Documentação Completa - Fase 5](https://github.com/joaosena19/fiap-12soat-projeto-fase-5-documentacao)

## Descrição

Microsserviço de Processamento em .NET, responsável por consumir diagramas enviados pelo serviço de Upload, analisá-los via LLM multimodal (Google Gemini) e publicar os resultados para o serviço de Relatório. Implementado como Worker (sem endpoints HTTP), com estratégia de fallback entre 4 modelos Gemini e resiliência via Polly. Implementado com Clean Architecture, PostgreSQL no Amazon RDS e Entity Framework Core. Executado em Kubernetes (EKS) com HPA.

## Tecnologias Utilizadas

- **.NET** - Runtime e Worker Service
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados (Amazon RDS)
- **MassTransit + Amazon SNS/SQS** - Mensageria assíncrona
- **Google Gemini** - LLM multimodal para análise de diagramas
- **Microsoft.Extensions.AI** - Abstração desacoplada do provider de LLM
- **Polly** - Resiliência e retry com backoff exponencial
- **Amazon S3** - Download dos arquivos de diagrama
- **Docker** - Containerização
- **Kubernetes** - Orquestração (Amazon EKS)
- **Terraform** - Provisionamento do banco de dados
- **New Relic** - Monitoramento e observabilidade
