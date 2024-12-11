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
import { mapTutorToForm } from "@/utils/other/mapper";
import UploadVideo from "./UploadVideo";
import { LogIn, Save } from "lucide-react";
import MajorAndSpecializationForm from "./MajorAndSpecializationSelect";
import {
  AddSubjectDTO,
  TutorDto,
  UpdateTutorInforDTO,
} from "@/utils/services/Api";
import { useUpdateTutor } from "@/hooks/use-tutor";
import QualificationLevelSelect from "@/app/(user)/tutors/[id]/components/QualificationLevelSelect";
import { toast } from "react-toastify";

const TutorProfileUpdateForm = ({ tutor }: { tutor?: TutorDto }) => {
  const { user } = useAppContext();
  const pathName = usePathname();
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
    defaultValues: tutor && mapTutorToForm(tutor),
  });

  console.log(form.formState.errors);
  console.log(form.getValues());

  const { mutateAsync: updateTutor, isLoading } = useUpdateTutor();

  async function onSubmit(values: TutorType) {
    try {
      const certificateUrls = await uploadFiles("certificates");
      const introVidUrl = await uploadVideo("video");
      const data: UpdateTutorInforDTO = {
        experience: values.experience,
        specialization: values.specialization,
        profileDescription: values.profileDescription,
        briefIntroduction: values.briefIntroduction,
        educationalLevelID: Number.parseInt(values.educationalLevel),
        major: values.major,
        videoUrl: introVidUrl,
        isAccepted: true,
        teachingLocation: values.teachingLocations.map((tl) => ({
          cityId: tl.cityId,
          districts: tl.districtIds.map((d) => ({
            districtId: d,
          })),
        })),
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
      };
      const res = await updateTutor(data);
      if (res.status == 201) {
        toast.success("Cập nhật thành công");
      }
      route.push("/user/tutor-profile");
    } catch (err) {
      toast.error("Cập nhật không thành công");
      console.log(err);
    }
  }

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
                  <SelectTeachingSubjects mode="update" />
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
          <div>
            <Button
              loading={isLoading}
              htmlType="submit"
              icon={<Save />}
              disabled={!form.formState.isDirty}
              type="primary"
            >
              Cập nhật
            </Button>
          </div>
        </div>
      </form>
    </Form>
  );
};

export default TutorProfileUpdateForm;
