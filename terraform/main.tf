# Data source para obter informacoes da VPC e subnets do terraform da infraestrutura
data "terraform_remote_state" "infra" {
  backend = "s3"
  config = {
    bucket = var.tfstate_bucket_name
    key    = var.infra_terraform_state_key
    region = var.aws_region
  }
}
