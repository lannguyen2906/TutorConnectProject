"use client";
import Image from "next/image";
import { mockTutor } from "./mockData";
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "@/components/ui/carousel";
import { BookOpen, Home, MapPin } from "lucide-react";
import { StarFilledIcon } from "@radix-ui/react-icons";
import { TutorHomePageDTO } from "@/utils/services/Api";
import { Tag } from "antd";
import { useRouter } from "next/navigation";

const SuggestedTutors = ({
  suggestedTutors,
}: {
  suggestedTutors: TutorHomePageDTO | undefined;
}) => {
  const router = useRouter();

  if (suggestedTutors?.totalRecordCount == 0) return <></>;

  return (
    <div className="my-8">
      <div className="text-2xl font-bold mb-5 mt-10 border-b-2">
        Bạn có thể sẽ quan tâm
      </div>
      <Carousel>
        <CarouselContent className="flex items-center">
          {suggestedTutors?.tutors?.map((tutor) => (
            <CarouselItem
              key={tutor.tutorId}
              className="basis-1/2 lg:basis-1/4"
            >
              <button
                onClick={() => router.push(`/tutors/${tutor.tutorId}`)}
                className="rounded-md overflow-hidden hover:bg-semiblueviolet hover:p-2 transition-all"
              >
                <Image
                  src={tutor.avatarUrl || mockTutor.avatarUrl}
                  alt={tutor.fullName || ""}
                  width={80}
                  height={80}
                  layout="responsive"
                />
                <h4 className="mt-2 text-lg text-left font-extrabold">
                  {tutor.fullName}
                </h4>
                <div className="flex text-sm justify-start items-center gap-2">
                  <MapPin size={16} />
                  <div className="whitespace-nowrap truncate w-[150px] text-left">
                    {tutor.teachingLocations
                      ?.flatMap((tc) => tc.cityName)
                      .join(", ")}
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  <BookOpen size={16} />
                  <div className="whitespace-nowrap truncate w-[150px] text-left">
                    {tutor.tutorSubjects
                      ?.map((ts) => ts.subject?.subjectName)
                      .join(", ")}
                  </div>
                </div>
                {tutor.rating ? (
                  <div className="flex items-center gap-2">
                    <StarFilledIcon color="Blueviolet" />
                    <div>
                      <span className="font-bold">{tutor.rating}/5 </span>
                      <span>({tutor.numberOfStudents})</span>
                    </div>
                  </div>
                ) : (
                  <Tag color="blueviolet">Mới</Tag>
                )}
              </button>
            </CarouselItem>
          ))}
        </CarouselContent>
        <CarouselPrevious />
        <CarouselNext />
      </Carousel>
    </div>
  );
};

export default SuggestedTutors;
