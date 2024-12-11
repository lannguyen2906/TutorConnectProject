import { Button, DatePicker, Upload, UploadFile } from "antd";
import { UploadCloud } from "lucide-react";
import { Controller, useFieldArray, useFormContext } from "react-hook-form";
import { TutorType } from "@/utils/schemaValidations/tutor.schema";
import { UseUploadFileResult } from "@/hooks/use-upload-file-easy";
import { FormLabel } from "@/components/ui/form";
import TextArea from "antd/es/input/TextArea";
import { date } from "zod";
import dayjs from "dayjs";

const UploadCertificates = ({
  handleFileChange,
  removeFile,
}: Pick<UseUploadFileResult, "handleFileChange" | "removeFile">) => {
  const { control, watch, setValue } = useFormContext();
  const { fields, append, update } = useFieldArray<
    Pick<TutorType, "certificates">
  >({
    control,
    name: "certificates",
  });

  const handleChangeCertificate = (fileList: UploadFile[]) => {
    fileList.forEach((file) => {
      if (file.status === "done") {
        const existingIndex = fields.findIndex((f) => f.uid == file.uid);
        const currentDes = watch(`certificates.${existingIndex}.description`);
        if (existingIndex >= 0) {
          update(existingIndex, {
            uid: file.uid,
            description: currentDes || "",
            imgUrl: file.thumbUrl || file.url || "",
            issueDate: "",
          });
        } else {
          // Otherwise, append a new certificate entry
          append({
            uid: file.uid,
            description: currentDes || "",
            imgUrl: file.thumbUrl || file.url || "",
            issueDate: "",
          });
        }
      }
      const currentFiles = watch("certificates");
      const newFiles = currentFiles.filter((f: UploadFile) =>
        fileList.find((file) => file.uid == f.uid)
      );

      setValue("certificates", newFiles);
    });
  };

  return (
    <div className="space-y-5">
      <Upload
        accept="image/*"
        onChange={(info) => {
          handleFileChange(info.fileList);
          handleChangeCertificate(info.fileList);
        }}
        onRemove={(remove) => {
          removeFile(remove.uid);
        }}
        listType="picture"
        defaultFileList={fields.map((_, index) => ({
          uid: fields[index]?.uid || "",
          name: fields[index]?.description || "",
          status: "done",
          url: fields[index]?.imgUrl || "",
        }))}
        itemRender={(_, file) => {
          const index = fields.findIndex((f) => f.uid == file.uid);
          return (
            <div className="flex flex-col gap-5 items-center my-5 shadow-Blueviolet shadow-sm p-5 rounded-lg">
              <div className="flex flex-col md:flex-row gap-2 w-full">
                <div className="w-full md:w-1/2">
                  <FormLabel required>Mô tả</FormLabel>
                  <Controller
                    control={control}
                    name={`certificates.${index > -1 && index}.description`}
                    render={({ field: { value, onChange } }) => (
                      <TextArea
                        value={value}
                        rows={1}
                        onChange={onChange}
                        placeholder="Nhập mô tả"
                      />
                    )}
                  />
                </div>
                <div className="w-full flex flex-col gap-2 md:w-1/4">
                  <FormLabel required>Ngày cấp bằng</FormLabel>
                  <Controller
                    control={control}
                    name={`certificates.${index > -1 && index}.issueDate`}
                    render={({ field: { value, onChange } }) => (
                      <DatePicker
                        value={value ? dayjs(value, "YYYY-MM-DD") : null} // Kiểm tra giá trị đầu vào
                        onChange={(date) => {
                          onChange(dayjs(date).format("YYYY-MM-DD"));
                        }}
                        placeholder="Chọn ngày"
                      />
                    )}
                  />
                </div>
                <div className="w-full flex flex-col gap-2 md:w-1/4">
                  <FormLabel>Ngày hết hạn</FormLabel>
                  <Controller
                    control={control}
                    name={`certificates.${index > -1 && index}.expiryDate`}
                    render={({ field: { value, onChange } }) => (
                      <DatePicker
                        value={value ? dayjs(value, "YYYY-MM-DD") : null} // Kiểm tra giá trị đầu vào
                        onChange={(date) => {
                          onChange(dayjs(date).format("YYYY-MM-DD"));
                        }}
                        placeholder="Chọn ngày"
                      />
                    )}
                  />
                </div>
              </div>
              <div className="w-full">{_}</div>
            </div>
          );
        }}
      >
        <Button type="dashed" icon={<UploadCloud />}>
          Thêm bằng cấp
        </Button>
      </Upload>
    </div>
  );
};

export default UploadCertificates;
