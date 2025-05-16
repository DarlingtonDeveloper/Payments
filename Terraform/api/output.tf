output "task_definition_revision" {
  description = "The latest revision of the ECS Task Definition"
  value       = module.ecs_payment.task_definition_revision
}
