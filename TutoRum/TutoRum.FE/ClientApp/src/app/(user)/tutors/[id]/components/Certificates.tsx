"use client";
import { Verified } from "lucide-react";
import { ITutor } from "./mockData";
import { Image, Tooltip } from "antd";
import { useAppContext } from "@/components/provider/app-provider";
import { VerifyButton } from "@/app/(admin)/admin/tutors/[id]/components/verify-buttons";
import { usePathname } from "next/navigation";
import { CertificateDTO, TutorDto } from "@/utils/services/Api";

type TutorCertificatesProps = Pick<TutorDto, "certificates" | "tutorId">;

const TutorCertificates = ({
  certificates,
  tutorId,
}: TutorCertificatesProps) => {
  const { user } = useAppContext();
  const pathName = usePathname();
  const isAdmin =
    user?.roles &&
    Array.isArray(user?.roles) &&
    user?.roles.includes("admin") &&
    pathName.startsWith("/admin");

  const displayCertificates = isAdmin
    ? certificates
    : certificates?.filter((c) => c.isVerified);
  if (displayCertificates?.length == 0) {
    return <></>;
  }

  const grouped: Record<string, any[]> = {};

  // Nhóm chứng chỉ theo năm
  certificates?.forEach((cert) => {
    const issueYear = new Date(cert.issueDate!).getFullYear();
    const expiryYear = cert.expiryDate
      ? new Date(cert.expiryDate).getFullYear()
      : null;

    const key = expiryYear ? `${issueYear} - ${expiryYear}` : `${issueYear}`;

    if (!grouped[key]) {
      grouped[key] = [];
    }
    grouped[key]?.push(cert);
  });

  return (
    <div className="my-8">
      <h2 className="text-2xl font-bold mb-5 mt-10 border-b-2">Bằng cấp</h2>
      <div className="ps-5">
        {Object.entries(grouped).length > 0 ? (
          <div className="space-y-6">
            {Object.entries(grouped).map(([yearRange, certs], index) => (
              <div
                key={index}
                className="flex items-center gap-5 bg-gray-100 p-4 rounded-md shadow"
              >
                <div className="font-bold text-Blueviolet w-1/5 text-xl border-e-2 pe-5">
                  {yearRange}
                </div>
                <div className="w-4/5">
                  {displayCertificates?.map((cert: CertificateDTO) => (
                    <div
                      key={cert.certificateId}
                      className="mb-2 flex justify-between items-center"
                    >
                      <div>
                        <div className="flex items-center justify-between">
                          <p className="font-bold">{cert.description}</p>
                          {cert.isVerified && (
                            <Tooltip title="Đã được kiểm duyệt">
                              <Verified className="text-Blueviolet" size={18} />
                            </Tooltip>
                          )}
                        </div>
                        <div className="text-sm text-gray-600">
                          Ngày cấp:{" "}
                          {new Date(cert.issueDate || "").toLocaleDateString()}
                          {cert.expiryDate && (
                            <>
                              {" "}
                              | Hết hạn:{" "}
                              {new Date(cert.expiryDate).toLocaleDateString()}
                            </>
                          )}
                        </div>
                      </div>
                      {isAdmin ? (
                        <>
                          <Image
                            src={cert.imgUrl || ""}
                            alt="certificate"
                            width={100}
                            height={100}
                          />
                          <VerifyButton
                            entityType={4}
                            guid={tutorId ?? null}
                            id={cert.certificateId ?? null}
                            isVerified={cert?.isVerified ?? null}
                          />
                        </>
                      ) : null}
                    </div>
                  ))}
                </div>
              </div>
            ))}
          </div>
        ) : (
          <div className="text-center">
            <h1 className="text-2xl font-bold">Kho học dữ liệu chưa cấp</h1>
          </div>
        )}
      </div>
    </div>
  );
};

export default TutorCertificates;
