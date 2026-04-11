variable "aws_region" {
  description = "Regiao da AWS onde os recursos serao criados"
  type        = string
  default     = "us-east-1"
}

variable "project_identifier" {
  description = "Identificador unico do projeto"
  type        = string
  default     = "fiap-12soat-fase5"
}

variable "db_identifier" {
  description = "Identificador da instancia PostgreSQL RDS para Processamento"
  type        = string
  default     = "fase5-processamento-database"
}

variable "db_name" {
  description = "Nome do banco de dados inicial"
  type        = string
  default     = "processamento_db"
}

variable "db_master_username" {
  description = "Username do usuario master do banco de dados"
  type        = string
  sensitive   = true
  default     = "processamento_admin"
}

variable "db_master_password" {
  description = "Senha do usuario master do banco de dados"
  type        = string
  sensitive   = true
}

variable "db_port" {
  description = "Porta do banco de dados PostgreSQL"
  type        = number
  default     = 5432
}

variable "backup_retention_period" {
  description = "Numero de dias para reter backups automaticos"
  type        = number
  default     = 1
}

variable "preferred_backup_window" {
  description = "Janela de tempo preferida para backups (UTC)"
  type        = string
  default     = "03:00-04:00"
}

variable "preferred_maintenance_window" {
  description = "Janela de tempo preferida para manutencao (UTC)"
  type        = string
  default     = "sun:04:00-sun:05:00"
}

variable "skip_final_snapshot" {
  description = "Determina se um snapshot final deve ser criado antes da delecao"
  type        = bool
  default     = true
}

variable "postgres_engine_version" {
  description = "Versao do engine PostgreSQL"
  type        = string
  default     = "17"
}

variable "postgres_instance_class" {
  description = "Classe da instancia PostgreSQL RDS"
  type        = string
  default     = "db.t3.micro"
}

variable "allocated_storage" {
  description = "Storage inicial alocado em GB"
  type        = number
  default     = 20
}

variable "max_allocated_storage" {
  description = "Storage maximo para auto-scaling em GB"
  type        = number
  default     = 50
}

variable "tfstate_bucket_name" {
  description = "Nome do bucket S3 onde esta o state da infraestrutura"
  type        = string
  default     = "fiap-12soat-fase5-joao-dainese"
}

variable "infra_terraform_state_key" {
  description = "Chave do state do Terraform da infraestrutura"
  type        = string
  default     = "infra/terraform.tfstate"
}
