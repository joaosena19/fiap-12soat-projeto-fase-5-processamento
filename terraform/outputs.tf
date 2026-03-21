output "db_instance_id" {
  description = "ID da instancia PostgreSQL RDS do Processamento"
  value       = aws_db_instance.processamento_postgres.id
}

output "db_instance_endpoint" {
  description = "Endpoint da instancia PostgreSQL RDS do Processamento"
  value       = aws_db_instance.processamento_postgres.endpoint
}

output "db_instance_address" {
  description = "Endereco (host) da instancia PostgreSQL RDS do Processamento"
  value       = aws_db_instance.processamento_postgres.address
}

output "db_instance_port" {
  description = "Porta da instancia PostgreSQL RDS do Processamento"
  value       = aws_db_instance.processamento_postgres.port
}

output "db_name" {
  description = "Nome do banco de dados do Processamento"
  value       = aws_db_instance.processamento_postgres.db_name
}

output "db_username" {
  description = "Username master do banco de dados do Processamento"
  value       = aws_db_instance.processamento_postgres.username
  sensitive   = true
}

output "db_security_group_id" {
  description = "ID do Security Group do PostgreSQL do Processamento"
  value       = aws_security_group.processamento_db_sg.id
}

output "db_subnet_group_name" {
  description = "Nome do DB Subnet Group do Processamento"
  value       = aws_db_subnet_group.processamento_subnet_group.name
}

# Connection string para uso nos ConfigMaps/Secrets do K8s
output "db_connection_string" {
  description = "Connection string do banco de dados do Processamento (sem senha)"
  value       = "Host=${aws_db_instance.processamento_postgres.address};Port=${aws_db_instance.processamento_postgres.port};Database=${aws_db_instance.processamento_postgres.db_name};Username=${aws_db_instance.processamento_postgres.username}"
  sensitive   = true
}
