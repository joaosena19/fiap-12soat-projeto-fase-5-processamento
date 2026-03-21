terraform {
  backend "s3" {
    bucket  = "fiap-12soat-fase5-joao-dainese"
    key     = "fase5/processamento-database/terraform.tfstate"
    region  = "us-east-1"
    encrypt = true
  }
}
