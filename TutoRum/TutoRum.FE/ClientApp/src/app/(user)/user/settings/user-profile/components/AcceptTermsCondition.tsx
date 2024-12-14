"use client";
import { Checkbox, Divider, List, Modal, Typography } from "antd";
import Paragraph from "antd/es/typography/Paragraph";
import Title from "antd/es/typography/Title";
import React, { useState } from "react";

const AcceptTermsCondition = ({
  isAgreed,
  setIsAgreed,
}: {
  isAgreed: boolean;
  setIsAgreed: (value: boolean) => void;
}) => {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <div className="space-x-2 my-2">
      <Checkbox checked={isAgreed} onClick={() => setIsAgreed(!isAgreed)} />
      <span className="text-sm">
        Tôi hoàn toàn đồng tình với các{" "}
        <button
          type="button"
          className="text-Blueviolet underline"
          onClick={() => setIsOpen(true)}
        >
          chính sách điều khoản và dịch vụ
        </button>
      </span>
      <Modal
        open={isOpen}
        onCancel={() => setIsOpen(false)}
        onOk={() => {
          setIsAgreed(true);
          setIsOpen(false);
        }}
        style={{ top: 20 }}
        cancelText="Đóng"
        okText="Đồng ý"
        getContainer={false}
      >
        <Typography>
          <Title level={3}>Điều khoản sử dụng</Title>

          <Divider />

          <Paragraph>
            Chào mừng bạn đến với hệ thống gia sư của chúng tôi! Khi sử dụng
            dịch vụ, bạn đồng ý tuân theo các điều khoản và quy định sau:
          </Paragraph>

          <List
            dataSource={[
              {
                title: "1. Quyền và trách nhiệm của gia sư",
                content:
                  "Gia sư có trách nhiệm cung cấp các thông tin chính xác và trung thực trong hồ sơ cá nhân. Gia sư phải tuân thủ cam kết về chất lượng giảng dạy và đảm bảo giờ giấc đã được thỏa thuận với học viên.",
              },
              {
                title: "2. Quyền và trách nhiệm của học viên",
                content:
                  "Học viên cần cung cấp thông tin chính xác khi đăng ký tài khoản và trong suốt quá trình học tập. Việc thanh toán phải được thực hiện đúng hạn theo quy định của hệ thống.",
              },
              {
                title: "3. Chính sách bảo mật",
                content:
                  "Hệ thống cam kết bảo mật thông tin cá nhân của gia sư và học viên. Thông tin của bạn sẽ chỉ được sử dụng trong phạm vi dịch vụ và không tiết lộ cho bên thứ ba mà không có sự đồng ý của bạn.",
              },
              {
                title: "4. Hủy bỏ và hoàn tiền",
                content:
                  "Việc hủy bỏ và hoàn tiền phải được thực hiện theo quy định của hệ thống. Gia sư và học viên cần tuân thủ các yêu cầu về thời gian hủy và hoàn tiền, nếu có, theo đúng thỏa thuận ban đầu.",
              },
            ]}
            renderItem={(item) => (
              <List.Item>
                <List.Item.Meta
                  title={<strong>{item.title}</strong>}
                  description={<Paragraph>{item.content}</Paragraph>}
                />
              </List.Item>
            )}
          />

          <Paragraph>
            Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi để
            được giải đáp. Chúng tôi mong muốn mang đến cho bạn trải nghiệm học
            tập tốt nhất!
          </Paragraph>
        </Typography>
      </Modal>
    </div>
  );
};

export default AcceptTermsCondition;
