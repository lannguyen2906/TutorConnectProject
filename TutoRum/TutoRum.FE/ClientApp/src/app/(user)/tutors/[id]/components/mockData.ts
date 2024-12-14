import { markdownSample } from "@/app/(user)/posts/[id]/markdown-sample";
import { DisplayPostType } from "@/utils/schemaValidations/post.schema";
import { TutorType } from "@/utils/schemaValidations/tutor.schema";

export interface IFeedback {
  feedbackId: string;
  fullName: string;
  classroom: string;
  createdDate: string;
  rating: number;
  comments: string;
  avatarUrl: string;
}

interface ITutorFeedbacks {
  avarageRating: number;
  totalFeedbacks: number;
  ratingsBreakdown: Record<string, number>;
  feedbacks: IFeedback[];
}

export interface ISubject {
  subjectId: number;
  subjectName: string;
}

interface ITutorSubjects {
  tutorSubjectId: number;
  description: string;
  rate: number;
  isVerify: boolean | null;
  subjectId: number;
  subjectName: string;
}

type SuggestedTutor = {
  tutorId: string;
  avatarUrl: string;
  fullName: string;
  rating: number;
  totalFeedbacks: number;
  subjects: string[];
  address: string;
};

interface TeachingLocation {
  city: string;
  cityId: string;
  districts: {
    districtId: string;
    districtName: string;
  }[];
}

export interface ITutor
  extends Omit<TutorType, "subjects" | "teachingLocations"> {
  fullName: string;
  avatarUrl: string;
  address: string;
  tutorFeedbacks: ITutorFeedbacks;
  tutorSubjects: ITutorSubjects[];
  suggestedTutors: SuggestedTutor[];
  teachingLocations: TeachingLocation[];
  rating: string;
  status: string;
  briefIntroduction: string;
  videoUrl: string;
}

export const mockPost: DisplayPostType = {
  categoryName: "Giáo dục",
  content: markdownSample,
  createdDate: new Date(),
  thumbnailUrl:
    "https://www.readingrockets.org/sites/default/files/styles/share_image/public/2023-05/a-z-tutoring.jpg?itok=1cuDsUkO",
  title: "Cách chọn gia sư phù hợp",
  writer: {
    avatarUrl:
      "https://img.freepik.com/free-photo/portrait-white-man-isolated_53876-40306.jpg",
    name: "Nguyễn Quang Tú",
  },
};

export const mockTutor: ITutor = {
  fullName: "Nguyễn Quang Tú",
  avatarUrl:
    "https://firebasestorage.googleapis.com/v0/b/tutorconnect-27339.appspot.com/o/avatar%2Fdefault-avatar-icon-of-social-media-user-vector.jpg?alt=media&token=27853f0b-a90f-424d-b4ca-526f00993ce0",
  address: "Hà Nội",
  videoUrl:
    "https://firebasestorage.googleapis.com/v0/b/tutorconnect-27339.appspot.com/o/video%2Fvideo%2F1729674204291_rc-upload-1729673926939-7?alt=media&token=332d3b7a-945e-4808-8bef-43555e714b90",
  briefIntroduction:
    "Hãy cùng nhau khám phá những cách học thú vị giúp bạn nắm vững kiến thức một cách dễ dàng và tự tin hơn!",
  certificates: [
    {
      uid: "1",
      issueDate: "2020-10-01T10:37:48.710Z",
      expiryDate: "2022-09-15T10:37:48.710Z",
      description: "IELTS 7.0",
      imgUrl:
        "https://firebasestorage.googleapis.com/v0/b/tutorconnect-27339.appspot.com/o/certificates%2Ffiles%2F1730107945674_rc-upload-1730107791069-5?alt=media&token=e3b64c4c-c55e-40cf-91ed-dd6b48be08f4",
      isVerified: true,
    },
    {
      uid: "2",
      issueDate: "2020-09-15T10:37:48.710Z",
      description: "TOEFL 100",
      imgUrl: mockPost.writer.avatarUrl,
      isVerified: false,
    },
    {
      uid: "1",
      issueDate: "2024-10-01T10:37:48.710Z",
      expiryDate: "2026-09-15T10:37:48.710Z",
      description: "IELTS 7.0",
      imgUrl:
        "https://firebasestorage.googleapis.com/v0/b/tutorconnect-27339.appspot.com/o/certificates%2Ffiles%2F1730107945674_rc-upload-1730107791069-5?alt=media&token=e3b64c4c-c55e-40cf-91ed-dd6b48be08f4",
      isVerified: true,
    },
  ],
  experience: "3",
  profileDescription: `## Giới thiệu về tôi 🎉

Xin chào các bạn! Mình là [Nguyễn Quang Tú] 👩‍🏫, một giáo viên với niềm đam mê giúp các bạn học sinh vượt qua những trở ngại trong học tập. Với **3 năm kinh nghiệm** 📚 giảng dạy và hướng dẫn, mình luôn cố gắng mang đến phương pháp học hiệu quả, phù hợp với từng học sinh.

Mình chuyên giảng dạy các môn:

- **Toán** ➕
- **Ngoại ngữ** 🌍

Với phương châm dạy học là "học đi đôi với hành" 💡, mình luôn cố gắng kết hợp giữa kiến thức lý thuyết và các bài tập thực tế, giúp các bạn không chỉ nắm vững lý thuyết mà còn biết cách áp dụng vào thực tiễn.

Hãy để mình đồng hành cùng bạn trên con đường chinh phục tri thức! 🚀
`,
  rating: "4.5",
  schedule: [
    {
      dayOfWeek: 1,
      freeTimes: [
        { startTime: "10:00", endTime: "11:00" },
        { startTime: "11:00", endTime: "12:00" },
      ],
    },
    {
      dayOfWeek: 2,
      freeTimes: [
        { startTime: "8:00", endTime: "11:00" },
        { startTime: "20:00", endTime: "22:00" },
      ],
    },
    {
      dayOfWeek: 3,
      freeTimes: [
        { startTime: "8:00", endTime: "11:00" },
        { startTime: "14:00", endTime: "17:00" },
        { startTime: "20:00", endTime: "22:00" },
      ],
    },
    {
      dayOfWeek: 4,
      freeTimes: [
        { startTime: "8:00", endTime: "11:00" },
        { startTime: "20:00", endTime: "22:00" },
      ],
    },
    {
      dayOfWeek: 5,
      freeTimes: [
        { startTime: "8:00", endTime: "11:00" },
        { startTime: "14:00", endTime: "17:00" },
        { startTime: "20:00", endTime: "22:00" },
      ],
    },
    {
      dayOfWeek: 6,
      freeTimes: [
        { startTime: "8:00", endTime: "11:00" },
        { startTime: "20:00", endTime: "22:00" },
      ],
    },
    {
      dayOfWeek: 7,
      freeTimes: [{ startTime: "8:00", endTime: "11:00" }],
    },
  ],
  specialization: "Kỹ thuật phần mềm",
  major: "Công nghệ thông tin",
  educationalLevel: "Sinh viên",
  tutorSubjects: [
    {
      description: "Toán 1",
      rate: 100000,
      tutorSubjectId: 1,
      isVerify: true,
      // numberOfRegistered: 10,

      subjectId: 1,
      subjectName: "Toán",
    },
    {
      description: "Toán 2",
      rate: 150000,
      tutorSubjectId: 2,
      isVerify: true,
      // numberOfRegistered: 5,

      subjectId: 1,
      subjectName: "Toán",
    },
  ],
  teachingLocations: [
    {
      cityId: "01",
      city: "Hà Nội",
      districts: [
        {
          districtId: "005",
          districtName: "Quận Cầu Giấy",
        },
        {
          districtId: "001",
          districtName: "Quận Ba Đình",
        },
      ],
    },
    {
      cityId: "33",
      city: "Hưng Yên",
      districts: [
        {
          districtId: "330",
          districtName: "Huyện Khoái Châu",
        },
      ],
    },
  ],
  status: "VERIFIED",
  tutorFeedbacks: {
    avarageRating: 4.9,
    totalFeedbacks: 49,
    ratingsBreakdown: {
      5: 47,
      4: 2,
      3: 0,
      2: 0,
      1: 0,
    },
    feedbacks: [
      {
        feedbackId: "1",
        fullName: "Piotr",
        createdDate: "August 18, 2022",
        rating: 5,
        comments: "sVery good teacher, motivating to work",
        avatarUrl: mockPost.writer.avatarUrl,
        classroom: "Toán 1",
      },
      {
        feedbackId: "1",
        fullName: "Jack",
        createdDate: "February 3, 2022",
        rating: 5,
        comments:
          "Very kind teacher with widely topic that you could talk with him.",
        avatarUrl: mockPost.writer.avatarUrl,
        classroom: "Toán 1",
      },
      {
        feedbackId: "1",
        fullName: "Asmaa",
        createdDate: "January 5, 2022",
        rating: 5,
        comments: "sGreat teacher",
        avatarUrl: mockPost.writer.avatarUrl,
        classroom: "Toán 1",
      },
      {
        feedbackId: "1",
        fullName: "Maria",
        createdDate: "December 10, 2021",
        rating: 5,
        comments:
          "Jose is an amazing tutor. He is flexible to focus on your needs. I like the fact that he is taking notes while you are speaking and then he explains to you what your mistakes were...",
        avatarUrl: mockPost.writer.avatarUrl,
        classroom: "Toán 1",
      },
      {
        feedbackId: "1",
        fullName: "Edilson",
        createdDate: "December 3, 2021",
        rating: 5,
        comments:
          "José was the best teacher I had here on Preply. In the first class, I was very nervous about my English skills and he helped me to build my confidence and trust myself. He's very...",
        avatarUrl: mockPost.writer.avatarUrl,
        classroom: "Toán 1",
      },
      {
        feedbackId: "1",
        fullName: "bruno",
        createdDate: "November 24, 2021",
        rating: 4,
        comments: "sGreat Teacher. I strongly recommend him.",
        avatarUrl: mockPost.writer.avatarUrl,
        classroom: "Toán 1",
      },
      {
        feedbackId: "1",
        fullName: "bruno",
        createdDate: "November 24, 2021",
        rating: 5,
        comments: "sGreat Teacher. I strongly recommend him.",
        avatarUrl: mockPost.writer.avatarUrl,
        classroom: "Toán 1",
      },
      {
        feedbackId: "1",
        fullName: "bruno",
        createdDate: "November 24, 2021",
        rating: 4,
        comments: "sGreat Teacher. I strongly recommend him.",
        avatarUrl: mockPost.writer.avatarUrl,
        classroom: "Toán 1",
      },
    ],
  },
  suggestedTutors: [
    {
      tutorId: "1",
      avatarUrl: mockPost.writer.avatarUrl,
      fullName: "Nguyễn Minh Đại",
      totalFeedbacks: 10,
      rating: 5,
      address: "Hà Nội",
      subjects: ["Toán 1", "Toán 2"],
    },
    {
      tutorId: "1",
      avatarUrl: mockPost.writer.avatarUrl,
      fullName: "Nguyễn Minh Đại",
      totalFeedbacks: 10,
      rating: 5,
      address: "Hà Nội",
      subjects: ["Toán 1", "Toán 2"],
    },
    {
      tutorId: "1",
      avatarUrl: mockPost.writer.avatarUrl,
      fullName: "Nguyễn Minh Đại",
      totalFeedbacks: 10,
      rating: 5,
      address: "Hà Nội",
      subjects: ["Toán 1", "Toán 2"],
    },
    {
      tutorId: "1",
      avatarUrl: mockPost.writer.avatarUrl,
      fullName: "Nguyễn Minh Đại",
      totalFeedbacks: 10,
      rating: 5,
      address: "Hà Nội",
      subjects: ["Toán 1", "Toán 2"],
    },
    {
      tutorId: "1",
      avatarUrl: mockPost.writer.avatarUrl,
      fullName: "Nguyễn Minh Đại",
      totalFeedbacks: 10,
      rating: 5,
      address: "Hà Nội",
      subjects: ["Toán 1", "Toán 2"],
    },
  ],
};
