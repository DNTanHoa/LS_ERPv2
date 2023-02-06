USE [ERPv2]
GO

/****** Object:  Table [dbo].[PermissionPolicyUserLoginInfo]    Script Date: 10/23/2021 11:59:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PermissionPolicyUserLoginInfo](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LoginProviderName] [nvarchar](450) NULL,
	[ProviderUserKey] [nvarchar](450) NULL,
	[UserForeignKey] [int] NOT NULL,
 CONSTRAINT [PK_PermissionPolicyUserLoginInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PermissionPolicyUserLoginInfo]  WITH CHECK ADD  CONSTRAINT [FK_PermissionPolicyUserLoginInfo_PermissionPolicyUser_UserForeignKey] FOREIGN KEY([UserForeignKey])
REFERENCES [dbo].[PermissionPolicyUser] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PermissionPolicyUserLoginInfo] CHECK CONSTRAINT [FK_PermissionPolicyUserLoginInfo_PermissionPolicyUser_UserForeignKey]
GO


USE [ERPv2]
GO

/****** Object:  Table [dbo].[PermissionPolicyUser]    Script Date: 10/23/2021 11:59:06 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PermissionPolicyUser](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[ChangePasswordOnFirstLogon] [bit] NOT NULL,
	[StoredPassword] [nvarchar](max) NULL,
	[Discriminator] [nvarchar](max) NOT NULL,
	[Phone] [nvarchar](1000) NULL,
	[Email] [nvarchar](1000) NULL,
	[NormalizedPhone] [nvarchar](1000) NULL,
	[NormalizedEmail] [nvarchar](1000) NULL,
	[FullName] [nvarchar](1000) NULL,
	[Supervisor] [nvarchar](1000) NULL,
 CONSTRAINT [PK_PermissionPolicyUser] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


USE [ERPv2]
GO

/****** Object:  Table [dbo].[PermissionPolicyTypePermissionObject]    Script Date: 10/23/2021 11:58:46 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PermissionPolicyTypePermissionObject](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TargetTypeFullName] [nvarchar](max) NULL,
	[RoleID] [int] NULL,
	[ReadState] [int] NULL,
	[WriteState] [int] NULL,
	[CreateState] [int] NULL,
	[DeleteState] [int] NULL,
	[NavigateState] [int] NULL,
 CONSTRAINT [PK_PermissionPolicyTypePermissionObject] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[PermissionPolicyTypePermissionObject]  WITH CHECK ADD  CONSTRAINT [FK_PermissionPolicyTypePermissionObject_PermissionPolicyRoleBase_RoleID] FOREIGN KEY([RoleID])
REFERENCES [dbo].[PermissionPolicyRoleBase] ([ID])
GO

ALTER TABLE [dbo].[PermissionPolicyTypePermissionObject] CHECK CONSTRAINT [FK_PermissionPolicyTypePermissionObject_PermissionPolicyRoleBase_RoleID]
GO


USE [ERPv2]
GO

/****** Object:  Table [dbo].[PermissionPolicyRolePermissionPolicyUser]    Script Date: 10/23/2021 11:58:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PermissionPolicyRolePermissionPolicyUser](
	[RolesID] [int] NOT NULL,
	[UsersID] [int] NOT NULL,
 CONSTRAINT [PK_PermissionPolicyRolePermissionPolicyUser] PRIMARY KEY CLUSTERED 
(
	[RolesID] ASC,
	[UsersID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PermissionPolicyRolePermissionPolicyUser]  WITH CHECK ADD  CONSTRAINT [FK_PermissionPolicyRolePermissionPolicyUser_PermissionPolicyRoleBase_RolesID] FOREIGN KEY([RolesID])
REFERENCES [dbo].[PermissionPolicyRoleBase] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PermissionPolicyRolePermissionPolicyUser] CHECK CONSTRAINT [FK_PermissionPolicyRolePermissionPolicyUser_PermissionPolicyRoleBase_RolesID]
GO

ALTER TABLE [dbo].[PermissionPolicyRolePermissionPolicyUser]  WITH CHECK ADD  CONSTRAINT [FK_PermissionPolicyRolePermissionPolicyUser_PermissionPolicyUser_UsersID] FOREIGN KEY([UsersID])
REFERENCES [dbo].[PermissionPolicyUser] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PermissionPolicyRolePermissionPolicyUser] CHECK CONSTRAINT [FK_PermissionPolicyRolePermissionPolicyUser_PermissionPolicyUser_UsersID]
GO

USE [ERPv2]
GO

/****** Object:  Table [dbo].[PermissionPolicyRoleBase]    Script Date: 10/23/2021 11:58:26 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PermissionPolicyRoleBase](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[IsAdministrative] [bit] NOT NULL,
	[CanEditModel] [bit] NOT NULL,
	[PermissionPolicy] [int] NOT NULL,
	[IsAllowPermissionPriority] [bit] NOT NULL,
	[Discriminator] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_PermissionPolicyRoleBase] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


USE [ERPv2]
GO

/****** Object:  Table [dbo].[PermissionPolicyObjectPermissionsObject]    Script Date: 10/23/2021 11:58:06 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PermissionPolicyObjectPermissionsObject](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Criteria] [nvarchar](max) NULL,
	[ReadState] [int] NULL,
	[WriteState] [int] NULL,
	[DeleteState] [int] NULL,
	[NavigateState] [int] NULL,
	[TypePermissionObjectID] [int] NULL,
 CONSTRAINT [PK_PermissionPolicyObjectPermissionsObject] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[PermissionPolicyObjectPermissionsObject]  WITH CHECK ADD  CONSTRAINT [FK_PermissionPolicyObjectPermissionsObject_PermissionPolicyTypePermissionObject_TypePermissionObjectID] FOREIGN KEY([TypePermissionObjectID])
REFERENCES [dbo].[PermissionPolicyTypePermissionObject] ([ID])
GO

ALTER TABLE [dbo].[PermissionPolicyObjectPermissionsObject] CHECK CONSTRAINT [FK_PermissionPolicyObjectPermissionsObject_PermissionPolicyTypePermissionObject_TypePermissionObjectID]
GO


USE [ERPv2]
GO

/****** Object:  Table [dbo].[PermissionPolicyNavigationPermissionObject]    Script Date: 10/23/2021 11:57:46 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PermissionPolicyNavigationPermissionObject](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[RoleID] [int] NULL,
	[ItemPath] [nvarchar](max) NULL,
	[TargetTypeFullName] [nvarchar](max) NULL,
	[NavigateState] [int] NULL,
 CONSTRAINT [PK_PermissionPolicyNavigationPermissionObject] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[PermissionPolicyNavigationPermissionObject]  WITH CHECK ADD  CONSTRAINT [FK_PermissionPolicyNavigationPermissionObject_PermissionPolicyRoleBase_RoleID] FOREIGN KEY([RoleID])
REFERENCES [dbo].[PermissionPolicyRoleBase] ([ID])
GO

ALTER TABLE [dbo].[PermissionPolicyNavigationPermissionObject] CHECK CONSTRAINT [FK_PermissionPolicyNavigationPermissionObject_PermissionPolicyRoleBase_RoleID]
GO

USE [ERPv2]
GO

/****** Object:  Table [dbo].[PermissionPolicyMemberPermissionsObject]    Script Date: 10/23/2021 11:57:26 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PermissionPolicyMemberPermissionsObject](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Members] [nvarchar](max) NULL,
	[Criteria] [nvarchar](max) NULL,
	[ReadState] [int] NULL,
	[WriteState] [int] NULL,
	[TypePermissionObjectID] [int] NULL,
 CONSTRAINT [PK_PermissionPolicyMemberPermissionsObject] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[PermissionPolicyMemberPermissionsObject]  WITH CHECK ADD  CONSTRAINT [FK_PermissionPolicyMemberPermissionsObject_PermissionPolicyTypePermissionObject_TypePermissionObjectID] FOREIGN KEY([TypePermissionObjectID])
REFERENCES [dbo].[PermissionPolicyTypePermissionObject] ([ID])
GO

ALTER TABLE [dbo].[PermissionPolicyMemberPermissionsObject] CHECK CONSTRAINT [FK_PermissionPolicyMemberPermissionsObject_PermissionPolicyTypePermissionObject_TypePermissionObjectID]
GO


USE [ERPv2]
GO

/****** Object:  Table [dbo].[PermissionPolicyActionPermissionObject]    Script Date: 10/23/2021 11:57:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PermissionPolicyActionPermissionObject](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[RoleID] [int] NULL,
	[ActionId] [nvarchar](max) NULL,
 CONSTRAINT [PK_PermissionPolicyActionPermissionObject] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[PermissionPolicyActionPermissionObject]  WITH CHECK ADD  CONSTRAINT [FK_PermissionPolicyActionPermissionObject_PermissionPolicyRoleBase_RoleID] FOREIGN KEY([RoleID])
REFERENCES [dbo].[PermissionPolicyRoleBase] ([ID])
GO

ALTER TABLE [dbo].[PermissionPolicyActionPermissionObject] CHECK CONSTRAINT [FK_PermissionPolicyActionPermissionObject_PermissionPolicyRoleBase_RoleID]
GO


USE [ERPv2]
GO

/****** Object:  Table [dbo].[ModulesInfo]    Script Date: 10/24/2021 12:03:01 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ModulesInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[AssemblyFileName] [nvarchar](max) NULL,
	[Version] [nvarchar](max) NULL,
	[IsMain] [bit] NOT NULL,
 CONSTRAINT [PK_ModulesInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

USE [ERPv2]
GO

/****** Object:  Table [dbo].[ModelDifferences]    Script Date: 10/24/2021 12:02:51 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ModelDifferences](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](max) NULL,
	[ContextId] [nvarchar](max) NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_ModelDifferences] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

USE [ERPv2]
GO

/****** Object:  Table [dbo].[ModelDifferenceAspects]    Script Date: 10/24/2021 12:02:41 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ModelDifferenceAspects](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Xml] [nvarchar](max) NULL,
	[OwnerID] [int] NULL,
 CONSTRAINT [PK_ModelDifferenceAspects] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ModelDifferenceAspects]  WITH CHECK ADD  CONSTRAINT [FK_ModelDifferenceAspects_ModelDifferences_OwnerID] FOREIGN KEY([OwnerID])
REFERENCES [dbo].[ModelDifferences] ([ID])
GO

ALTER TABLE [dbo].[ModelDifferenceAspects] CHECK CONSTRAINT [FK_ModelDifferenceAspects_ModelDifferences_OwnerID]
GO


--- Migrate data

USE [ERPv2]
GO

--- Delete old user

DELETE FROM [dbo].[PermissionPolicyUser]

INSERT INTO [dbo].[PermissionPolicyUser]
           ([UserName]
           ,[IsActive]
           ,[ChangePasswordOnFirstLogon]
           ,[StoredPassword]
           ,[Discriminator]
           ,[Phone]
           ,[Email]
           ,[NormalizedPhone]
           ,[NormalizedEmail]
           ,[FullName]
           ,[Supervisor])
SELECT [USER].[UserName]
      ,[USER].[IsActive]
      ,[USER].[ChangePasswordOnFirstLogon]
      ,[USER].[StoredPassword]
      ,'ApplicationUser'
	  ,[PERSON].[Phone]
	  ,[PERSON].[Email]
	  ,UPPER([PERSON].[Phone])
	  ,UPPER([PERSON].[Email])
	  ,[PERSON].[Contact]
	  ,[SUPERVISOR].[UserName]
  FROM 
	[LS_ERPv1.5].[dbo].[PermissionPolicyUser] [USER]
	LEFT JOIN [LS_ERPv1.5].[dbo].[Person] [PERSON] ON [USER].[Oid] = [Person].[User]
	LEFT JOIN [LS_ERPv1.5].[dbo].[PermissionPolicyUser] [SUPERVISOR] ON [SUPERVISOR].[Oid] = [PERSON].[Suppervisor]


--- Delete old provider

DELETE FROM [dbo].[PermissionPolicyUserLoginInfo]

USE [ERPv2]
GO

--- Add login provider

INSERT INTO [dbo].[PermissionPolicyUserLoginInfo]
           ([LoginProviderName]
           ,[ProviderUserKey]
           ,[UserForeignKey])
     SELECT
		 'Password'
		,[USER].[Id]
		,[USER].[Id]
	 FROM
		[dbo].[PermissionPolicyUser] [USER]
GO

--- Delete old role data
DELETE FROM [PermissionPolicyNavigationPermissionObject]
DELETE FROM [PermissionPolicyMemberPermissionsObject]
DELETE FROM [PermissionPolicyTypePermissionObject]
DELETE FROM [PermissionPolicyTypePermissionObject]
DELETE FROM [PermissionPolicyObjectPermissionsObject]
DELETE FROM [PermissionPolicyRoleBase]

--- Migrate PermissionPolicyRole
INSERT INTO [dbo].[PermissionPolicyRoleBase]
           ([Name]
           ,[IsAdministrative]
           ,[CanEditModel]
           ,[PermissionPolicy]
           ,[IsAllowPermissionPriority]
           ,[Discriminator])
     SELECT
       [Name]
      ,[IsAdministrative]
      ,[CanEditModel]
      ,[PermissionPolicy]
      ,[ObjectType]
	  ,'PermissionPolicyRole'
  FROM [LS_ERPv1.5].[dbo].[PermissionPolicyRole]






