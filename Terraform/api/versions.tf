provider "aws" {
  region = var.region
  ignore_tags {
    key_prefixes = ["cai:catalog"]
  }
  default_tags {
    tags = {
      Environment = var.environment
      Workflow    = var.project
    }
  }
}

provider "alks" {
  url = "https://alks.coxautoinc.com/rest"
  ignore_tags {
    key_prefixes = ["cai:catalog"]
  }
}

terraform {
  required_version = ">= 1.5.6"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
    alks = {
      source  = "Cox-Automotive/alks"
      version = ">= 2.8.0"
    }
  }
  backend "s3" {}
}
