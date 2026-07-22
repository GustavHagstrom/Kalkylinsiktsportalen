using BlazorServer.Identity;

namespace BlazorServer.Modules.Projects.Domain;

public sealed class Project
{
    private Project(
        ProjectId id,
        OrganizationId organizationId,
        UserId createdBy,
        DateTimeOffset createdAt,
        string name, 
        string? description)
    {
        Id = id;
        OrganizationId = organizationId;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
        Name = name;
    }

    public ProjectId Id { get; }

    public OrganizationId OrganizationId { get; }

    public UserId CreatedBy { get; }

    public DateTimeOffset CreatedAt { get; }

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public static Project Create(
        ProjectId id,
        OrganizationId organizationId,
        UserId createdBy,
        DateTimeOffset createdAt,
        string name,
        string? description = null)
    {
        if (id.IsEmpty)
        {
            throw new ProjectDomainException("project.id.required", "Project id is required.");
        }

        if (organizationId.IsEmpty)
        {
            throw new ProjectDomainException("project.organization.required", "Organization id is required.");
        }

        if (createdBy.IsEmpty)
        {
            throw new ProjectDomainException("project.createdBy.required", "Created by user id is required.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ProjectDomainException("project.name.required", "Project name is required.");
        }

        return new Project(id, organizationId, createdBy, createdAt, name.Trim(), description?.Trim());
    }
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ProjectDomainException("project.name.required", "Project name is required.");
        }
        Name = newName.Trim();
    }
    public void UpdateDescription(string? newDescription)
    {
        Description = newDescription?.Trim();
    }
}
