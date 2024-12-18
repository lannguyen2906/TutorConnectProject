USE [master]
GO
/****** Object:  Database [tutor2]    Script Date: 10/8/2024 5:16:08 AM ******/
CREATE DATABASE [tutor2]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'tutor2', FILENAME = N'E:\Microsoft SQL Server\MSSQL15.LANNT\MSSQL\DATA\tutor2.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'tutor2_log', FILENAME = N'E:\Microsoft SQL Server\MSSQL15.LANNT\MSSQL\DATA\tutor2_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [tutor2] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [tutor2].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [tutor2] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [tutor2] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [tutor2] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [tutor2] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [tutor2] SET ARITHABORT OFF 
GO
ALTER DATABASE [tutor2] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [tutor2] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [tutor2] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [tutor2] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [tutor2] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [tutor2] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [tutor2] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [tutor2] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [tutor2] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [tutor2] SET  DISABLE_BROKER 
GO
ALTER DATABASE [tutor2] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [tutor2] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [tutor2] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [tutor2] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [tutor2] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [tutor2] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [tutor2] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [tutor2] SET RECOVERY FULL 
GO
ALTER DATABASE [tutor2] SET  MULTI_USER 
GO
ALTER DATABASE [tutor2] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [tutor2] SET DB_CHAINING OFF 
GO
ALTER DATABASE [tutor2] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [tutor2] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [tutor2] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [tutor2] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'tutor2', N'ON'
GO
ALTER DATABASE [tutor2] SET QUERY_STORE = OFF
GO
USE [tutor2]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 10/8/2024 5:16:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Admin]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admin](
	[AdminId] [uniqueidentifier] NOT NULL,
	[Position] [nvarchar](255) NULL,
	[HireDate] [date] NULL,
	[Salary] [decimal](18, 2) NULL,
	[SupervisorId] [int] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[MetaDescription] [nvarchar](256) NULL,
	[MetaKeyword] [nvarchar](256) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_Admin] PRIMARY KEY CLUSTERED 
(
	[AdminId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [uniqueidentifier] NOT NULL,
	[Fullname] [nvarchar](max) NULL,
	[DOB] [date] NULL,
	[Gender] [bit] NULL,
	[AvatarUrl] [nchar](250) NULL,
	[AddressID] [nvarchar](256) NULL,
	[AddressDetail] [nvarchar](256) NULL,
	[Status] [nvarchar](256) NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](15) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [uniqueidentifier] NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bill]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bill](
	[BillID] [int] IDENTITY(1,1) NOT NULL,
	[BillingEntryID] [int] NULL,
	[Discount] [decimal](18, 2) NULL,
	[StartDate] [datetime] NULL,
	[Description] [nvarchar](max) NULL,
	[TotalBill] [decimal](18, 2) NULL,
	[Status] [nvarchar](50) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[MetaDescription] [nvarchar](255) NULL,
 CONSTRAINT [PK_Bill] PRIMARY KEY CLUSTERED 
(
	[BillID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BillingEntry]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BillingEntry](
	[BillingEntryID] [int] IDENTITY(1,1) NOT NULL,
	[TutorLearnerSubjectId] [int] NULL,
	[Rate] [decimal](18, 2) NULL,
	[StartDateTime] [datetime] NULL,
	[EndDateTime] [datetime] NULL,
	[Description] [nvarchar](max) NULL,
	[TotalAmount] [decimal](18, 2) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[MetaDescription] [nvarchar](255) NULL,
 CONSTRAINT [PK_BillingEntry] PRIMARY KEY CLUSTERED 
(
	[BillingEntryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Certificate]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Certificate](
	[CertificateId] [int] IDENTITY(1,1) NOT NULL,
	[TutorId] [uniqueidentifier] NULL,
	[ImgUrl] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[Status] [nvarchar](50) NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Certificate] PRIMARY KEY CLUSTERED 
(
	[CertificateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Classroom]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Classroom](
	[ClassroomId] [int] IDENTITY(1,1) NOT NULL,
	[TutorLearnerSubjectId] [int] NULL,
	[MetaKeyword] [nvarchar](255) NULL,
	[MetaDescription] [nvarchar](255) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Classroom] PRIMARY KEY CLUSTERED 
(
	[ClassroomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feedback]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedback](
	[FeedbackId] [int] IDENTITY(1,1) NOT NULL,
	[TutorLearnerSubjectId] [int] NULL,
	[Rating] [decimal](3, 2) NULL,
	[Comments] [nvarchar](max) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Feedback] PRIMARY KEY CLUSTERED 
(
	[FeedbackId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payment](
	[PaymentID] [int] IDENTITY(1,1) NOT NULL,
	[BillID] [int] NULL,
	[AmountPaid] [decimal](18, 2) NULL,
	[PaymentMethod] [nvarchar](50) NULL,
	[PaymentDate] [datetime] NULL,
	[Currency] [nvarchar](10) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[PaymentStatus] [nvarchar](50) NULL,
	[MetaDescription] [nvarchar](255) NULL,
 CONSTRAINT [PK_Payment] PRIMARY KEY CLUSTERED 
(
	[PaymentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Post]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Post](
	[PostId] [int] IDENTITY(1,1) NOT NULL,
	[AdminId] [uniqueidentifier] NULL,
	[Title] [nvarchar](255) NULL,
	[Content] [nvarchar](max) NULL,
	[Subcontent] [nvarchar](max) NULL,
	[Thumbnail] [nvarchar](255) NULL,
	[Status] [nvarchar](50) NULL,
	[PostType] [int] NOT NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[MetaDescription] [nvarchar](256) NULL,
	[MetaKeyword] [nvarchar](256) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_Post] PRIMARY KEY CLUSTERED 
(
	[PostId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PostCategory]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PostCategory](
	[PostType] [int] IDENTITY(1,1) NOT NULL,
	[PostName] [nchar](100) NULL,
 CONSTRAINT [PK_PostCategory] PRIMARY KEY CLUSTERED 
(
	[PostType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QualificationLevel]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QualificationLevel](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Level] [nvarchar](max) NULL,
 CONSTRAINT [PK_QualificationLevel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Schedule]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Schedule](
	[TutorId] [uniqueidentifier] NOT NULL,
	[LearnerId] [uniqueidentifier] NULL,
	[TutorLearnerSubjectId] [int] NULL,
	[DayOfWeek] [int] NULL,
	[StartTime] [time](7) NULL,
	[EndTime] [time](7) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[MetaKeyword] [nvarchar](256) NULL,
	[MetaDescription] [nvarchar](256) NULL,
	[ScheduleId] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_Schedule] PRIMARY KEY CLUSTERED 
(
	[ScheduleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Subject]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subject](
	[SubjectId] [int] IDENTITY(1,1) NOT NULL,
	[SubjectName] [nvarchar](255) NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedDate] [datetime] NULL,
	[MetaKeyword] [nvarchar](255) NULL,
	[MetaDescription] [nvarchar](255) NULL,
 CONSTRAINT [PK_Subject] PRIMARY KEY CLUSTERED 
(
	[SubjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TeachingLocation]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TeachingLocation](
	[TeachingLocationId] [int] IDENTITY(1,1) NOT NULL,
	[CityId] [int] NULL,
	[DistrictId] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[MetaKeyword] [nvarchar](255) NULL,
	[MetaDescription] [nvarchar](255) NULL,
 CONSTRAINT [PK_TeachingLocation] PRIMARY KEY CLUSTERED 
(
	[TeachingLocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tutor]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tutor](
	[TutorId] [uniqueidentifier] NOT NULL,
	[Experience] [int] NULL,
	[Specialization] [nvarchar](255) NULL,
	[Rating] [decimal](3, 2) NULL,
	[Status] [nvarchar](50) NULL,
	[ProfileDescription] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[MetaKeyword] [nvarchar](255) NULL,
	[MetaDescription] [nvarchar](255) NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Tutor] PRIMARY KEY CLUSTERED 
(
	[TutorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TutorLearnerSubject]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TutorLearnerSubject](
	[TutorLearnerSubjectId] [int] IDENTITY(1,1) NOT NULL,
	[TutorSubjectId] [int] NULL,
	[LearnerId] [uniqueidentifier] NULL,
	[Location] [nvarchar](255) NULL,
	[ContractUrl] [nvarchar](255) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Status] [nvarchar](50) NULL,
 CONSTRAINT [PK_TutorLearnerSubject] PRIMARY KEY CLUSTERED 
(
	[TutorLearnerSubjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TutorRequest]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TutorRequest](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[RequestSummary] [nvarchar](max) NULL,
	[TeachingLocation] [nvarchar](max) NULL,
	[NumberOfStudents] [int] NOT NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[TimePerSession] [time](7) NOT NULL,
	[Subject] [nvarchar](max) NULL,
	[StudentGender] [nvarchar](max) NULL,
	[TutorGender] [nvarchar](max) NULL,
	[Fee] [decimal](18, 2) NOT NULL,
	[SessionsPerWeek] [int] NOT NULL,
	[DetailedDescription] [nvarchar](max) NULL,
	[TutorQualificationId] [int] NULL,
	[AspNetUserId] [uniqueidentifier] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[MetaDescription] [nvarchar](256) NULL,
	[MetaKeyword] [nvarchar](256) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_TutorRequest] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TutorSubject]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TutorSubject](
	[TutorSubjectId] [int] IDENTITY(1,1) NOT NULL,
	[TutorId] [uniqueidentifier] NULL,
	[SubjectId] [int] NULL,
	[Rate] [decimal](18, 2) NULL,
	[Description] [nvarchar](max) NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedDate] [datetime] NULL,
	[MetaKeyword] [nvarchar](255) NULL,
	[MetaDescription] [nvarchar](255) NULL,
 CONSTRAINT [PK_TutorSubject] PRIMARY KEY CLUSTERED 
(
	[TutorSubjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TutorTeachingLocations]    Script Date: 10/8/2024 5:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TutorTeachingLocations](
	[TutorId] [uniqueidentifier] NOT NULL,
	[TeachingLocationId] [int] NOT NULL,
 CONSTRAINT [PK_TutorTeachingLocations] PRIMARY KEY CLUSTERED 
(
	[TutorId] ASC,
	[TeachingLocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241003080531_InitialCreate', N'6.0.33')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241004030530_CreatePostCategoryAndUpdatePost', N'6.0.33')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241004055604_addAudiableForPosts', N'6.0.33')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241005125435_addTutorRequests', N'6.0.33')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241006114630_scheduletable', N'6.0.33')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241006115324_scheduletableRelationship', N'6.0.33')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241006123336_scheduletableRelationshipModel', N'6.0.33')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241006124738_UpdateScheduleIdToInt', N'6.0.33')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241006131209_UpdateScheduleIdToIntchange', N'6.0.33')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241006134303_Update123', N'6.0.33')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241008091914_CreateAdminAspNetUserRelation', N'6.0.33')
GO
INSERT [dbo].[Admin] ([AdminId], [Position], [HireDate], [Salary], [SupervisorId], [CreatedBy], [CreatedDate], [MetaDescription], [MetaKeyword], [UpdatedBy], [UpdatedDate]) VALUES (N'07c72cfb-b270-4bfd-8224-a6b80b3033ab', N'string', CAST(N'2024-10-03' AS Date), CAST(0.00 AS Decimal(18, 2)), 0, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Admin] ([AdminId], [Position], [HireDate], [Salary], [SupervisorId], [CreatedBy], [CreatedDate], [MetaDescription], [MetaKeyword], [UpdatedBy], [UpdatedDate]) VALUES (N'8c2daecd-747a-4441-9dcf-fc770ac6f9fc', N'string', CAST(N'2024-10-04' AS Date), CAST(0.00 AS Decimal(18, 2)), 0, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'825add0d-06d3-4166-b33c-2608e5995419', N'learner', N'LEARNER', NULL)
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'b4d64b01-4bdb-4a8f-ae33-44e7fc10d0c1', N'admin', N'ADMIN', NULL)
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'aa17e1bd-1032-4129-b2d8-5f303e172b35', N'tutor', N'TUTOR', NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'8d2cf291-5c93-4508-a6bb-069bb1a2bcc6', N'825add0d-06d3-4166-b33c-2608e5995419')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'64710641-acb8-41df-b000-44239e02faae', N'825add0d-06d3-4166-b33c-2608e5995419')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'050a57d3-e257-4b74-9436-7fe4a49988db', N'825add0d-06d3-4166-b33c-2608e5995419')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'07c72cfb-b270-4bfd-8224-a6b80b3033ab', N'825add0d-06d3-4166-b33c-2608e5995419')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'8c2daecd-747a-4441-9dcf-fc770ac6f9fc', N'b4d64b01-4bdb-4a8f-ae33-44e7fc10d0c1')
GO
INSERT [dbo].[AspNetUsers] ([Id], [Fullname], [DOB], [Gender], [AvatarUrl], [AddressID], [AddressDetail], [Status], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'8d2cf291-5c93-4508-a6bb-069bb1a2bcc6', N'user@example.com', NULL, NULL, NULL, NULL, NULL, NULL, N'user@example.com', N'USER@EXAMPLE.COM', N'user@example.com', N'USER@EXAMPLE.COM', 0, N'AQAAAAEAACcQAAAAEL3j+n+AWU6JX6ADU6+joS4N9q2J7fiHAzJBjLFUQ4h00iGHLaM+J5+RSVeq/uTP/Q==', N'W5LR6O2UKQNEWM6UPFPT4MXS4FWFTCNL', N'69a527a1-cc35-44a4-9351-c3c5ff51602a', NULL, 0, 0, NULL, 1, 0)
INSERT [dbo].[AspNetUsers] ([Id], [Fullname], [DOB], [Gender], [AvatarUrl], [AddressID], [AddressDetail], [Status], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'64710641-acb8-41df-b000-44239e02faae', N'use129r@example.com', NULL, NULL, NULL, NULL, NULL, NULL, N'use129r@example.com', N'USE129R@EXAMPLE.COM', N'use129r@example.com', N'USE129R@EXAMPLE.COM', 0, N'AQAAAAEAACcQAAAAEMfxvc4x8/47ky45s34DFAqa0Oyk/dR0YpZaZ/P3LsylIULsxgATxUOVokt4oPUE6w==', N'ND42MFZIJQEPQVHCNFLKBZ7F6CT2GST5', N'3c8fc0ac-152c-446c-9f1e-894c4e8a1300', NULL, 0, 0, NULL, 1, 0)
INSERT [dbo].[AspNetUsers] ([Id], [Fullname], [DOB], [Gender], [AvatarUrl], [AddressID], [AddressDetail], [Status], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'050a57d3-e257-4b74-9436-7fe4a49988db', N'lan.nguyen@evizi.com', NULL, NULL, NULL, NULL, NULL, NULL, N'lan.nguyen@evizi.com', N'LAN.NGUYEN@EVIZI.COM', N'lan.nguyen@evizi.com', N'LAN.NGUYEN@EVIZI.COM', 1, N'AQAAAAEAACcQAAAAEBytD6sebEvT1SIJz/zQ9ufjEyjvNkyEEmyNal77Z9u4+ZrkPOFuoJdjI3FvV4wW0g==', N'66J6XHCCA7STB2OLIBEGDKHYMGUAB2QX', N'62443178-dff5-4f52-b3d6-805aae7abd8f', NULL, 0, 0, NULL, 1, 0)
INSERT [dbo].[AspNetUsers] ([Id], [Fullname], [DOB], [Gender], [AvatarUrl], [AddressID], [AddressDetail], [Status], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'07c72cfb-b270-4bfd-8224-a6b80b3033ab', N'lannt29062002@gmail.com', NULL, NULL, NULL, NULL, NULL, NULL, N'lannt29062002@gmail.com', N'LANNT29062002@GMAIL.COM', N'lannt29062002@gmail.com', N'LANNT29062002@GMAIL.COM', 1, N'AQAAAAEAACcQAAAAEBZxq/dkvigmfYH8X8kBwDoETyXdOeOIapTyft6KShl40F5D2n0czyvZzXYuJZiujQ==', N'SBXDTYF4GIVOH6MHUKG5C4EDPUB5T7TS', N'de922b41-f2b6-415e-8281-540d6f4db9ad', NULL, 0, 0, NULL, 1, 0)
INSERT [dbo].[AspNetUsers] ([Id], [Fullname], [DOB], [Gender], [AvatarUrl], [AddressID], [AddressDetail], [Status], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'8c2daecd-747a-4441-9dcf-fc770ac6f9fc', N'lannthe160821@fpt.edu.vn', NULL, NULL, NULL, NULL, NULL, NULL, N'lannthe160821@fpt.edu.vn', N'LANNTHE160821@FPT.EDU.VN', N'lannthe160821@fpt.edu.vn', N'LANNTHE160821@FPT.EDU.VN', 1, N'AQAAAAEAACcQAAAAEMuT2kzDNfrEHOuf0zoi0mJaOnQzZtKDgRAwLej04VN5zcgSjiNwvhwhl/0mYPBWHA==', N'IZ3FOXKJMFNNQM3NC7TLBTS775LUU4SE', N'ad52b422-af3b-427f-b9ef-315a61283796', NULL, 0, 0, NULL, 1, 0)
GO
SET IDENTITY_INSERT [dbo].[Certificate] ON 

INSERT [dbo].[Certificate] ([CertificateId], [TutorId], [ImgUrl], [Description], [Status], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'07c72cfb-b270-4bfd-8224-a6b80b3033ab', N'https://example.com/certificate1.jpg', N'Bachelor''s Degree in Mathematics', NULL, NULL, CAST(N'2024-10-06T14:00:40.767' AS DateTime), NULL, NULL)
INSERT [dbo].[Certificate] ([CertificateId], [TutorId], [ImgUrl], [Description], [Status], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'07c72cfb-b270-4bfd-8224-a6b80b3033ab', N'https://example.com/certificate2.jpg', N'Certified Math Tutor', NULL, NULL, CAST(N'2024-10-06T14:00:40.807' AS DateTime), NULL, NULL)
INSERT [dbo].[Certificate] ([CertificateId], [TutorId], [ImgUrl], [Description], [Status], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, N'8d2cf291-5c93-4508-a6bb-069bb1a2bcc6', N'https://example.com/certificate1.jpg', N'Bachelor''s Degree in Mathematics', NULL, NULL, CAST(N'2024-10-06T14:04:12.357' AS DateTime), NULL, NULL)
INSERT [dbo].[Certificate] ([CertificateId], [TutorId], [ImgUrl], [Description], [Status], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (4, N'8d2cf291-5c93-4508-a6bb-069bb1a2bcc6', N'https://example.com/certificate2.jpg', N'Certified Math Tutor', NULL, NULL, CAST(N'2024-10-06T14:04:12.377' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[Certificate] OFF
GO
SET IDENTITY_INSERT [dbo].[Post] ON 

INSERT [dbo].[Post] ([PostId], [AdminId], [Title], [Content], [Subcontent], [Thumbnail], [Status], [PostType], [CreatedBy], [CreatedDate], [MetaDescription], [MetaKeyword], [UpdatedBy], [UpdatedDate]) VALUES (1, N'8c2daecd-747a-4441-9dcf-fc770ac6f9fc', N'POst1', N'dww22d1d', N'asdw', N'https://localhost:7026/swagger/index.html', N'dang hien thi', 8, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Post] ([PostId], [AdminId], [Title], [Content], [Subcontent], [Thumbnail], [Status], [PostType], [CreatedBy], [CreatedDate], [MetaDescription], [MetaKeyword], [UpdatedBy], [UpdatedDate]) VALUES (4, N'8c2daecd-747a-4441-9dcf-fc770ac6f9fc', N'string', N'string', N'string', N'string', N'1', 8, N'8c2daecd-747a-4441-9dcf-fc770ac6f9fc', CAST(N'2024-10-04T06:20:10.3098028' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Post] ([PostId], [AdminId], [Title], [Content], [Subcontent], [Thumbnail], [Status], [PostType], [CreatedBy], [CreatedDate], [MetaDescription], [MetaKeyword], [UpdatedBy], [UpdatedDate]) VALUES (5, N'8c2daecd-747a-4441-9dcf-fc770ac6f9fc', N'stringkk', N'string', N'string', N'string', N'Hidden', 8, N'8c2daecd-747a-4441-9dcf-fc770ac6f9fc', CAST(N'2024-10-04T06:20:57.1913158' AS DateTime2), NULL, NULL, N'8c2daecd-747a-4441-9dcf-fc770ac6f9fc', CAST(N'2024-10-04T06:21:35.3414363' AS DateTime2))
INSERT [dbo].[Post] ([PostId], [AdminId], [Title], [Content], [Subcontent], [Thumbnail], [Status], [PostType], [CreatedBy], [CreatedDate], [MetaDescription], [MetaKeyword], [UpdatedBy], [UpdatedDate]) VALUES (6, N'8c2daecd-747a-4441-9dcf-fc770ac6f9fc', N'stringkk', N'string', N'string', N'string', N'Hidden', 8, N'8c2daecd-747a-4441-9dcf-fc770ac6f9fc', CAST(N'2024-10-04T06:22:34.0404489' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Post] ([PostId], [AdminId], [Title], [Content], [Subcontent], [Thumbnail], [Status], [PostType], [CreatedBy], [CreatedDate], [MetaDescription], [MetaKeyword], [UpdatedBy], [UpdatedDate]) VALUES (7, N'8c2daecd-747a-4441-9dcf-fc770ac6f9fc', N'string', N'string', N'string', N'string', N'string', 8, N'8c2daecd-747a-4441-9dcf-fc770ac6f9fc', CAST(N'2024-10-04T06:24:12.3646047' AS DateTime2), NULL, NULL, N'8c2daecd-747a-4441-9dcf-fc770ac6f9fc', CAST(N'2024-10-04T06:52:38.3210989' AS DateTime2))
SET IDENTITY_INSERT [dbo].[Post] OFF
GO
SET IDENTITY_INSERT [dbo].[PostCategory] ON 

INSERT [dbo].[PostCategory] ([PostType], [PostName]) VALUES (8, N'Giới Thiệu                                                                                          ')
INSERT [dbo].[PostCategory] ([PostType], [PostName]) VALUES (9, N'Giáo Giục                                                                                           ')
INSERT [dbo].[PostCategory] ([PostType], [PostName]) VALUES (10, N'Công Nghẹ                                                                                           ')
INSERT [dbo].[PostCategory] ([PostType], [PostName]) VALUES (11, N'Tin Tức                                                                                             ')
SET IDENTITY_INSERT [dbo].[PostCategory] OFF
GO
SET IDENTITY_INSERT [dbo].[Schedule] ON 

INSERT [dbo].[Schedule] ([TutorId], [LearnerId], [TutorLearnerSubjectId], [DayOfWeek], [StartTime], [EndTime], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [MetaKeyword], [MetaDescription], [ScheduleId]) VALUES (N'8d2cf291-5c93-4508-a6bb-069bb1a2bcc6', NULL, NULL, 1, CAST(N'09:00:00' AS Time), CAST(N'11:00:00' AS Time), NULL, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Schedule] ([TutorId], [LearnerId], [TutorLearnerSubjectId], [DayOfWeek], [StartTime], [EndTime], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [MetaKeyword], [MetaDescription], [ScheduleId]) VALUES (N'8d2cf291-5c93-4508-a6bb-069bb1a2bcc6', NULL, NULL, 3, CAST(N'14:00:00' AS Time), CAST(N'16:00:00' AS Time), NULL, NULL, NULL, NULL, NULL, NULL, 2)
INSERT [dbo].[Schedule] ([TutorId], [LearnerId], [TutorLearnerSubjectId], [DayOfWeek], [StartTime], [EndTime], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [MetaKeyword], [MetaDescription], [ScheduleId]) VALUES (N'8d2cf291-5c93-4508-a6bb-069bb1a2bcc6', NULL, NULL, 5, CAST(N'09:00:00' AS Time), CAST(N'11:00:00' AS Time), NULL, NULL, NULL, NULL, NULL, NULL, 3)
SET IDENTITY_INSERT [dbo].[Schedule] OFF
GO
SET IDENTITY_INSERT [dbo].[TeachingLocation] ON 

INSERT [dbo].[TeachingLocation] ([TeachingLocationId], [CityId], [DistrictId], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [MetaKeyword], [MetaDescription]) VALUES (3, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[TeachingLocation] ([TeachingLocationId], [CityId], [DistrictId], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [MetaKeyword], [MetaDescription]) VALUES (6, 1, 5, CAST(N'2024-10-06T13:55:15.990' AS DateTime), NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[TeachingLocation] ([TeachingLocationId], [CityId], [DistrictId], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [MetaKeyword], [MetaDescription]) VALUES (7, 1, 8, CAST(N'2024-10-06T13:55:16.553' AS DateTime), NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[TeachingLocation] OFF
GO
INSERT [dbo].[Tutor] ([TutorId], [Experience], [Specialization], [Rating], [Status], [ProfileDescription], [CreatedDate], [UpdatedDate], [MetaKeyword], [MetaDescription], [CreatedBy], [UpdatedBy]) VALUES (N'8d2cf291-5c93-4508-a6bb-069bb1a2bcc6', 2, N'Mathematics', CAST(4.50 AS Decimal(3, 2)), NULL, N'Experienced tutor in high school mathematics', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Tutor] ([TutorId], [Experience], [Specialization], [Rating], [Status], [ProfileDescription], [CreatedDate], [UpdatedDate], [MetaKeyword], [MetaDescription], [CreatedBy], [UpdatedBy]) VALUES (N'050a57d3-e257-4b74-9436-7fe4a49988db', 0, N'string', CAST(0.00 AS Decimal(3, 2)), NULL, N'string', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Tutor] ([TutorId], [Experience], [Specialization], [Rating], [Status], [ProfileDescription], [CreatedDate], [UpdatedDate], [MetaKeyword], [MetaDescription], [CreatedBy], [UpdatedBy]) VALUES (N'07c72cfb-b270-4bfd-8224-a6b80b3033ab', 2, N'Mathematics', CAST(4.50 AS Decimal(3, 2)), NULL, N'Experienced tutor in high school mathematics', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Tutor] ([TutorId], [Experience], [Specialization], [Rating], [Status], [ProfileDescription], [CreatedDate], [UpdatedDate], [MetaKeyword], [MetaDescription], [CreatedBy], [UpdatedBy]) VALUES (N'8c2daecd-747a-4441-9dcf-fc770ac6f9fc', 2, N'Mathematics', CAST(4.50 AS Decimal(3, 2)), NULL, N'Experienced tutor in high school mathematics', NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[TutorTeachingLocations] ([TutorId], [TeachingLocationId]) VALUES (N'8d2cf291-5c93-4508-a6bb-069bb1a2bcc6', 6)
INSERT [dbo].[TutorTeachingLocations] ([TutorId], [TeachingLocationId]) VALUES (N'8d2cf291-5c93-4508-a6bb-069bb1a2bcc6', 7)
INSERT [dbo].[TutorTeachingLocations] ([TutorId], [TeachingLocationId]) VALUES (N'050a57d3-e257-4b74-9436-7fe4a49988db', 3)
INSERT [dbo].[TutorTeachingLocations] ([TutorId], [TeachingLocationId]) VALUES (N'07c72cfb-b270-4bfd-8224-a6b80b3033ab', 6)
INSERT [dbo].[TutorTeachingLocations] ([TutorId], [TeachingLocationId]) VALUES (N'07c72cfb-b270-4bfd-8224-a6b80b3033ab', 7)
INSERT [dbo].[TutorTeachingLocations] ([TutorId], [TeachingLocationId]) VALUES (N'8c2daecd-747a-4441-9dcf-fc770ac6f9fc', 6)
INSERT [dbo].[TutorTeachingLocations] ([TutorId], [TeachingLocationId]) VALUES (N'8c2daecd-747a-4441-9dcf-fc770ac6f9fc', 7)
GO
/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [EmailIndex]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Bill_BillingEntryID]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_Bill_BillingEntryID] ON [dbo].[Bill]
(
	[BillingEntryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BillingEntry_TutorLearnerSubjectId]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_BillingEntry_TutorLearnerSubjectId] ON [dbo].[BillingEntry]
(
	[TutorLearnerSubjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Classroom_TutorLearnerSubjectId]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_Classroom_TutorLearnerSubjectId] ON [dbo].[Classroom]
(
	[TutorLearnerSubjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Feedback_TutorLearnerSubjectId]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_Feedback_TutorLearnerSubjectId] ON [dbo].[Feedback]
(
	[TutorLearnerSubjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Payment_BillID]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_Payment_BillID] ON [dbo].[Payment]
(
	[BillID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Post_AdminId]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_Post_AdminId] ON [dbo].[Post]
(
	[AdminId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Schedule_LearnerId]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_Schedule_LearnerId] ON [dbo].[Schedule]
(
	[LearnerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Schedule_TutorId]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_Schedule_TutorId] ON [dbo].[Schedule]
(
	[TutorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Schedule_TutorLearnerSubjectId]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_Schedule_TutorLearnerSubjectId] ON [dbo].[Schedule]
(
	[TutorLearnerSubjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TutorLearnerSubject_LearnerId]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_TutorLearnerSubject_LearnerId] ON [dbo].[TutorLearnerSubject]
(
	[LearnerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TutorLearnerSubject_TutorSubjectId]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_TutorLearnerSubject_TutorSubjectId] ON [dbo].[TutorLearnerSubject]
(
	[TutorSubjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TutorRequest_AspNetUserId]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_TutorRequest_AspNetUserId] ON [dbo].[TutorRequest]
(
	[AspNetUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TutorRequest_TutorQualificationId]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_TutorRequest_TutorQualificationId] ON [dbo].[TutorRequest]
(
	[TutorQualificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TutorSubject_SubjectId]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_TutorSubject_SubjectId] ON [dbo].[TutorSubject]
(
	[SubjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TutorSubject_TutorId]    Script Date: 10/8/2024 5:16:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_TutorSubject_TutorId] ON [dbo].[TutorSubject]
(
	[TutorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AspNetUsers] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Admin]  WITH CHECK ADD  CONSTRAINT [FK_Admin_AspNetUsers_AdminId] FOREIGN KEY([AdminId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Admin] CHECK CONSTRAINT [FK_Admin_AspNetUsers_AdminId]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Bill]  WITH CHECK ADD  CONSTRAINT [FK__Bill__BillingEnt__52593CB8] FOREIGN KEY([BillingEntryID])
REFERENCES [dbo].[BillingEntry] ([BillingEntryID])
GO
ALTER TABLE [dbo].[Bill] CHECK CONSTRAINT [FK__Bill__BillingEnt__52593CB8]
GO
ALTER TABLE [dbo].[BillingEntry]  WITH CHECK ADD  CONSTRAINT [FK__BillingEn__Tutor__4F7CD00D] FOREIGN KEY([TutorLearnerSubjectId])
REFERENCES [dbo].[TutorLearnerSubject] ([TutorLearnerSubjectId])
GO
ALTER TABLE [dbo].[BillingEntry] CHECK CONSTRAINT [FK__BillingEn__Tutor__4F7CD00D]
GO
ALTER TABLE [dbo].[Certificate]  WITH CHECK ADD  CONSTRAINT [FK_Certificate_Tutor] FOREIGN KEY([TutorId])
REFERENCES [dbo].[Tutor] ([TutorId])
GO
ALTER TABLE [dbo].[Certificate] CHECK CONSTRAINT [FK_Certificate_Tutor]
GO
ALTER TABLE [dbo].[Classroom]  WITH CHECK ADD  CONSTRAINT [FK__Classroom__Tutor__4CA06362] FOREIGN KEY([TutorLearnerSubjectId])
REFERENCES [dbo].[TutorLearnerSubject] ([TutorLearnerSubjectId])
GO
ALTER TABLE [dbo].[Classroom] CHECK CONSTRAINT [FK__Classroom__Tutor__4CA06362]
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD  CONSTRAINT [FK__Feedback__TutorL__49C3F6B7] FOREIGN KEY([TutorLearnerSubjectId])
REFERENCES [dbo].[TutorLearnerSubject] ([TutorLearnerSubjectId])
GO
ALTER TABLE [dbo].[Feedback] CHECK CONSTRAINT [FK__Feedback__TutorL__49C3F6B7]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FK__Payment__BillID__5535A963] FOREIGN KEY([BillID])
REFERENCES [dbo].[Bill] ([BillID])
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FK__Payment__BillID__5535A963]
GO
ALTER TABLE [dbo].[Post]  WITH CHECK ADD  CONSTRAINT [FK_Post_Admin] FOREIGN KEY([AdminId])
REFERENCES [dbo].[Admin] ([AdminId])
GO
ALTER TABLE [dbo].[Post] CHECK CONSTRAINT [FK_Post_Admin]
GO
ALTER TABLE [dbo].[Post]  WITH CHECK ADD  CONSTRAINT [FK_Post_PostCategory_PostType] FOREIGN KEY([PostType])
REFERENCES [dbo].[PostCategory] ([PostType])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Post] CHECK CONSTRAINT [FK_Post_PostCategory_PostType]
GO
ALTER TABLE [dbo].[Schedule]  WITH CHECK ADD  CONSTRAINT [FK_Schedule_AspNetUsers_LearnerId] FOREIGN KEY([LearnerId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Schedule] CHECK CONSTRAINT [FK_Schedule_AspNetUsers_LearnerId]
GO
ALTER TABLE [dbo].[Schedule]  WITH CHECK ADD  CONSTRAINT [FK_Schedule_Tutor_TutorId] FOREIGN KEY([TutorId])
REFERENCES [dbo].[Tutor] ([TutorId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Schedule] CHECK CONSTRAINT [FK_Schedule_Tutor_TutorId]
GO
ALTER TABLE [dbo].[Schedule]  WITH CHECK ADD  CONSTRAINT [FK_Schedule_TutorLearnerSubject_TutorLearnerSubjectId] FOREIGN KEY([TutorLearnerSubjectId])
REFERENCES [dbo].[TutorLearnerSubject] ([TutorLearnerSubjectId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Schedule] CHECK CONSTRAINT [FK_Schedule_TutorLearnerSubject_TutorLearnerSubjectId]
GO
ALTER TABLE [dbo].[Tutor]  WITH CHECK ADD  CONSTRAINT [FK_Tutor_AspNetUsers] FOREIGN KEY([TutorId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Tutor] CHECK CONSTRAINT [FK_Tutor_AspNetUsers]
GO
ALTER TABLE [dbo].[TutorLearnerSubject]  WITH CHECK ADD  CONSTRAINT [FK__TutorLear__Tutor__45F365D3] FOREIGN KEY([TutorSubjectId])
REFERENCES [dbo].[TutorSubject] ([TutorSubjectId])
GO
ALTER TABLE [dbo].[TutorLearnerSubject] CHECK CONSTRAINT [FK__TutorLear__Tutor__45F365D3]
GO
ALTER TABLE [dbo].[TutorRequest]  WITH CHECK ADD  CONSTRAINT [FK_TutorRequest_AspNetUsers_AspNetUserId] FOREIGN KEY([AspNetUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[TutorRequest] CHECK CONSTRAINT [FK_TutorRequest_AspNetUsers_AspNetUserId]
GO
ALTER TABLE [dbo].[TutorRequest]  WITH CHECK ADD  CONSTRAINT [FK_TutorRequest_QualificationLevel_TutorQualificationId] FOREIGN KEY([TutorQualificationId])
REFERENCES [dbo].[QualificationLevel] ([Id])
GO
ALTER TABLE [dbo].[TutorRequest] CHECK CONSTRAINT [FK_TutorRequest_QualificationLevel_TutorQualificationId]
GO
ALTER TABLE [dbo].[TutorSubject]  WITH CHECK ADD  CONSTRAINT [FK__TutorSubj__Subje__4316F928] FOREIGN KEY([SubjectId])
REFERENCES [dbo].[Subject] ([SubjectId])
GO
ALTER TABLE [dbo].[TutorSubject] CHECK CONSTRAINT [FK__TutorSubj__Subje__4316F928]
GO
ALTER TABLE [dbo].[TutorSubject]  WITH CHECK ADD  CONSTRAINT [FK_TutorSubject_Tutor] FOREIGN KEY([TutorId])
REFERENCES [dbo].[Tutor] ([TutorId])
GO
ALTER TABLE [dbo].[TutorSubject] CHECK CONSTRAINT [FK_TutorSubject_Tutor]
GO
ALTER TABLE [dbo].[TutorTeachingLocations]  WITH CHECK ADD  CONSTRAINT [FK_TutorTeachingLocations_TeachingLocation] FOREIGN KEY([TeachingLocationId])
REFERENCES [dbo].[TeachingLocation] ([TeachingLocationId])
GO
ALTER TABLE [dbo].[TutorTeachingLocations] CHECK CONSTRAINT [FK_TutorTeachingLocations_TeachingLocation]
GO
ALTER TABLE [dbo].[TutorTeachingLocations]  WITH CHECK ADD  CONSTRAINT [FK_TutorTeachingLocations_Tutor] FOREIGN KEY([TutorId])
REFERENCES [dbo].[Tutor] ([TutorId])
GO
ALTER TABLE [dbo].[TutorTeachingLocations] CHECK CONSTRAINT [FK_TutorTeachingLocations_Tutor]
GO
USE [master]
GO
ALTER DATABASE [tutor2] SET  READ_WRITE 
GO
