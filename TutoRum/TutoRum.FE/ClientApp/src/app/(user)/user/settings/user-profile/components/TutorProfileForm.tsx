"use client";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { TutorSchema, TutorType } from "@/utils/schemaValidations/tutor.schema";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button, Input } from "antd";
import TextArea from "antd/es/input/TextArea";
import React, { useState } from "react";
import { useForm } from "react-hook-form";
import SelectTeachingLocations from "./SelectTeachingLocations";
import SelectTeachingSubjects from "./SelectTeachingSubjects";
import SelectFreeSchedule from "./SelectFreeSchedule";
import { useAppContext } from "@/components/provider/app-provider";
import { cn } from "@/lib/utils";
import { usePathname, useRouter } from "next/navigation";
import MDEditor from "@uiw/react-md-editor";
import UploadCertificates from "./UploadCertificates";
import useUploadFileEasy from "@/hooks/use-upload-file-easy";
import { ITutor } from "@/app/(user)/tutors/[id]/components/mockData";
import { mapTutorToForm } from "@/utils/other/mapper";
import UploadVideo from "./UploadVideo";
import { LogIn, Save } from "lucide-react";
import MajorAndSpecializationForm from "./MajorAndSpecializationSelect";
import { AddSubjectDTO, AddTutorDTO, TutorDto } from "@/utils/services/Api";
import { useRegisterTutor } from "@/hooks/use-tutor";
import QualificationLevelSelect from "@/app/(user)/tutors/[id]/components/QualificationLevelSelect";
import AcceptTermsCondition from "./AcceptTermsCondition";
import { toast } from "react-toastify";

const TutorProfileForm = ({ tutor }: { tutor?: TutorDto }) => {
  const { user } = useAppContext();
  const pathName = usePathname();
  const [isAgreed, setIsAgreed] = useState(false);
  const route = useRouter();
  const {
    handleFileChange,
    removeFile,
    handleVideoChange,
    uploadFiles,
    uploadVideo,
  } = useUploadFileEasy();
  const form = useForm<TutorType>({
    resolver: zodResolver(TutorSchema),
    defaultValues: {
      teachingLocations: [{ cityId: undefined }],
      subjects: [{ subjectType: undefined }],
      schedule: [
        {
          dayOfWeek: 1,
          freeTimes: [{ startTime: "8:00", endTime: "16:00" }],
        },
        {
          dayOfWeek: 2,
          freeTimes: [{ startTime: "8:00", endTime: "16:00" }],
        },
        {
          dayOfWeek: 3,
          freeTimes: [{ startTime: "8:00", endTime: "16:00" }],
        },
        {
          dayOfWeek: 4,
          freeTimes: [{ startTime: "8:00", endTime: "16:00" }],
        },
        {
          dayOfWeek: 5,
          freeTimes: [{ startTime: "8:00", endTime: "16:00" }],
        },
        {
          dayOfWeek: 6,
          freeTimes: [{ startTime: "8:00", endTime: "16:00" }],
        },
        {
          dayOfWeek: 7,
          freeTimes: [{ startTime: "8:00", endTime: "16:00" }],
        },
      ],
    },
  });

  const { mutateAsync: registerTutor, isLoading } = useRegisterTutor();

  async function onSubmit(values: TutorType) {
    try {
      const certificateUrls = await uploadFiles("certificates");
      const introVidUrl = await uploadVideo("video");
      const data: AddTutorDTO = {
        briefIntroduction: values.briefIntroduction,
        educationalLevelID: Number.parseInt(values.educationalLevel),
        profileDescription: values.profileDescription,
        experience: values.experience,
        major: values.major,
        specialization: values.specialization,
        teachingLocation: values.teachingLocations.map((tl) => ({
          cityId: tl.cityId,
          districts: tl.districtIds.map((d) => ({
            districtId: d,
          })),
        })),
        schedule: values.schedule,
        subjects: values.subjects.map((s) => ({
          subjectName: s.subjectName,
          rateRangeId: s.learnerType,
          subjectType: s.subjectType,
          description: s.description,
          rate: s.rate,
        })) as AddSubjectDTO[],
        certificates: values.certificates?.map((c, i) => ({
          imgUrl: certificateUrls[i],
          description: c.description,
          expiryDate: c.expiryDate,
          issueDate: c.issueDate,
        })),
        videoUrl: introVidUrl,
        isAccepted: isAgreed,
      };
      const res = await registerTutor(data);
      if (res.status == 201) {
        toast.success("Đăng ký thành công");
      }
      route.push("/user/tutor-profile");
    } catch (err) {
      toast.error("Đăng ký không thành công");
      console.log(err);
    }
  }

  console.log(form.formState.errors);

  return (
    <Form {...form}>
      <form className="relative" onSubmit={form.handleSubmit(onSubmit)}>
        {!user && (
          <div className="z-10 absolute w-full h-full flex flex-col justify-center items-center text-lg font-bold">
            <span>Bạn cần đăng nhập để sử dụng tính năng này</span>
            <Button
              icon={<LogIn />}
              onClick={() => route.push("/login")}
              className="mt-4"
              type="primary"
            >
              Đăng nhập
            </Button>
          </div>
        )}

        <div className={cn("space-y-8", !user && "blur-sm")}>
          <div className="flex flex-col xl:flex-row w-full justify-between gap-10">
            <div className="flex flex-col w-full xl:w-1/2 gap-5">
              <div className="w-full flex flex-col md:flex-row gap-5">
                <MajorAndSpecializationForm form={form} />
              </div>
              <div className="w-full flex flex-col md:flex-row gap-5">
                <FormField
                  control={form.control}
                  name="educationalLevel"
                  render={({ field }) => (
                    <FormItem className="w-full md:w-1/2">
                      <FormLabel required>Trình độ</FormLabel>
                      <FormControl>
                        <div className="w-full">
                          <QualificationLevelSelect field={field} />
                        </div>
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="experience"
                  render={({ field }) => (
                    <FormItem className="w-full md:w-1/2">
                      <FormLabel required>
                        Số năm kinh nghiệm giảng dạy
                      </FormLabel>
                      <FormControl>
                        <Input type="number" placeholder="1" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
            </div>
            <div className="flex flex-col w-full xl:w-1/2 gap-5">
              <FormField
                control={form.control}
                name="briefIntroduction"
                render={({ field }) => (
                  <FormItem className="w-full">
                    <FormLabel required>Giới thiệu ngắn gọn</FormLabel>
                    <FormControl>
                      <TextArea
                        rows={5}
                        placeholder="vidu@gmail.com"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>
          </div>
          <div className="flex flex-col-reverse xl:flex-row w-full justify-between gap-10">
            <FormField
              control={form.control}
              name="teachingLocations"
              render={({ field }) => (
                <FormItem className="w-full xl:w-1/2">
                  <FormLabel required>Khu vực bạn có thể dạy</FormLabel>
                  <FormControl>
                    <SelectTeachingLocations />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="profileDescription"
              render={({ field }) => (
                <FormItem data-color-mode="light" className="w-full xl:w-1/2">
                  <FormLabel required>Giới thiệu chi tiết</FormLabel>
                  <FormControl>
                    <MDEditor value={field.value} onChange={field.onChange} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>
          <FormField
            control={form.control}
            name="subjects"
            render={({ field }) => (
              <FormItem className="w-full">
                <FormLabel required>Môn học bạn có thể dạy</FormLabel>
                <FormControl>
                  <SelectTeachingSubjects />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="schedule"
            render={({ field }) => (
              <FormItem className="w-full">
                <FormLabel required>Khung giờ bạn có thể dạy</FormLabel>
                <FormControl>
                  <SelectFreeSchedule />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <div className="flex flex-col lg:flex-row gap-10">
            <FormField
              control={form.control}
              name="certificates"
              render={({ field }) => (
                <FormItem className="w-full">
                  <FormLabel>Bằng cấp</FormLabel>
                  <FormControl>
                    <UploadCertificates
                      handleFileChange={handleFileChange}
                      removeFile={removeFile}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormItem className="w-full">
              <FormLabel>Video giới thiệu bản thân</FormLabel>
              <FormControl>
                <UploadVideo handleVideoChange={handleVideoChange} />
              </FormControl>
              <FormMessage />
            </FormItem>
          </div>
          {!tutor && (
            <div>
              <Button
                loading={isLoading}
                htmlType="submit"
                icon={<Save />}
                disabled={!isAgreed}
                type="primary"
              >
                Đăng ký
              </Button>
              <AcceptTermsCondition
                isAgreed={isAgreed}
                setIsAgreed={setIsAgreed}
              />
            </div>
          )}
        </div>
      </form>
    </Form>
  );
};

export default TutorProfileForm;
