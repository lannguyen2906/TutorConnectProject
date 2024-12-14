import {
  useChooseTutor,
  useInterestTutor,
  useListTutorRegisteredDetail,
} from "@/hooks/use-tutor-request";
import { TutorInTutorRequestDTO } from "@/utils/services/Api";
import { Button, Modal, Table, Tag } from "antd";
import { ColumnsType } from "antd/es/table";
import { CheckCircle, Eye, Heart } from "lucide-react";
import React from "react";
import { toast } from "react-toastify";

const InterestButton = ({
  record,
  tutorRequestId,
}: {
  record: TutorInTutorRequestDTO;
  tutorRequestId: number;
}) => {
  const { mutateAsync, isLoading } = useInterestTutor(tutorRequestId);

  const handleInterest = async () => {
    try {
      const data = await mutateAsync(record.tutorId!);
      if (data.status === 201) {
        toast.success("Đã gửi thông tin qua mail gia sư");
      }
    } catch (err) {
      toast.error("Lỗi khi gửi thông tin");
    }
  };

  return (
    <Button
      title={
        record.isVerified
          ? "Đã gửi thông tin cho gia sư này"
          : "Gửi thông tin cho gia sư này"
      }
      style={{ color: "pink" }}
      icon={
        record.isVerified ? (
          <Heart fill="pink" size={16} />
        ) : (
          <Heart size={16} />
        )
      }
      disabled={record.isVerified || false}
      type="text"
      loading={isLoading}
      onClick={handleInterest}
    />
  );
};

const ChooseButton = ({
  record,
  tutorRequestId,
}: {
  record: TutorInTutorRequestDTO;
  tutorRequestId: number;
}) => {
  const { mutateAsync, isLoading } = useChooseTutor(tutorRequestId);
  const [isModalOpen, setIsModalOpen] = React.useState(false);

  const handleChoose = async () => {
    try {
      const data = await mutateAsync(record.tutorId!);
      if (data.status === 201) {
        toast.success("Chọn gia sư thành công");
        setIsModalOpen(false);
      }
    } catch (err) {
      toast.error("Lỗi khi chọn gia sư");
    }
  };

  return (
    <>
      <Button
        title="Chọn gia sư này"
        style={{ color: "green" }}
        icon={<CheckCircle size={16} />}
        type="text"
        onClick={() => setIsModalOpen(true)}
      />
      <Modal
        title="Chọn gia sư"
        open={isModalOpen}
        onCancel={() => setIsModalOpen(false)}
        okText="Chọn"
        onOk={handleChoose}
        confirmLoading={isLoading}
        cancelText="Hủy"
      >
        Bạn đồng ý chọn gia sư này chứ?
      </Modal>
    </>
  );
};

const RegisteredTutorsTable = ({
  tutorRequestId,
}: {
  tutorRequestId: number;
}) => {
  // Mock hook thay thế khi gọi API
  const { data: tutors, isLoading } =
    useListTutorRegisteredDetail(tutorRequestId);
  const columns: ColumnsType<TutorInTutorRequestDTO> = [
    {
      title: "Họ và tên",
      dataIndex: "name",
      key: "name",
    },
    {
      title: "Chuyên môn",
      dataIndex: "specialization",
      key: "specialization",
    },
    {
      title: "Hành động",
      render: (_, record) => (
        <div className="flex gap-2 items-center">
          <a
            href={`/tutors/${record.tutorId}`}
            target="_blank"
            rel="noopener noreferrer"
          >
            <Button
              title="Xem chi tiết"
              type="text"
              icon={<Eye color="Blueviolet" size={16} />}
            />
          </a>
          <InterestButton tutorRequestId={tutorRequestId} record={record} />
          <ChooseButton tutorRequestId={tutorRequestId} record={record} />
        </div>
      ),
    },
  ];

  return (
    <div style={{ padding: "16px" }}>
      <Table
        columns={columns}
        dataSource={tutors?.tutors || []}
        loading={isLoading}
        rowKey={"tutorId"} // Đặt key cho từng hàng
        bordered
        pagination={false}
        title={() => `Danh sách gia sư đã nhận yêu cầu #${tutorRequestId}`}
      />
    </div>
  );
};

export default RegisteredTutorsTable;
