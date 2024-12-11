"use client";
import React, { useState } from "react";
import { Button, Tag, Alert } from "antd";
import TutorCompareModel from "./TutorCompareModel";
import { useRouter } from "next/navigation";
import { Trash2 } from "lucide-react";

const CompareModal = ({
  visible,
  onClose,
  selectedTutors,
  removeTutor,
}: {
  visible: boolean;
  onClose: () => void;
  selectedTutors: string[];
  removeTutor: (tutorId: string) => void;
}) => {
  const router = useRouter();
  const [showAlert, setShowAlert] = useState(false);

  const handleCompareClick = () => {
    if (selectedTutors.length === 2) {
      const [id1, id2] = selectedTutors;
      window.open(`/tutor-compare?tutor1=${id1}&tutor2=${id2}`, "_blank");
    } else {
      setShowAlert(true);
    }
  };

  if (!visible) return null; // Chỉ hiển thị khi visible = true

  return (
    <div
      style={{
        position: "fixed",
        bottom: 0,
        left: 0,
        width: "400px",
        background: "#fff",
        boxShadow: "0px 0px 10px rgba(0, 0, 0, 0.2)",
        padding: "16px",
        zIndex: 1000, // Đảm bảo modal hiển thị nổi
        borderRadius: "8px",
      }}
    >
      <div className="flex justify-between items-center mb-4">
        <h3 className="text-lg font-semibold">So sánh gia sư</h3>
        <div
          onClick={onClose}
          className="w-6 h-6 bg-red-500 text-black flex items-center justify-center rounded-full cursor-pointer border-"
        >
          x
        </div>
      </div>
      <div>
        <h4 className="mb-2">Các gia sư đã chọn:</h4>
        {selectedTutors.map((id) => (
          <div key={id} className="flex items-center space-x-2 mb-2">
            <TutorCompareModel params={{ tutorId: id }} />
            <div
              onClick={() => removeTutor(id)}
              className="text-red-500 hover:text-red-700 cursor-pointer"
            >
              <Trash2 className="w-5 h-5" />
            </div>
          </div>
        ))}
      </div>

      {showAlert && (
        <Alert
          message="Vui lòng chọn 2 gia sư để so sánh."
          type="warning"
          showIcon
          closable
          onClose={() => setShowAlert(false)}
          className="mt-4"
        />
      )}

      <div className="flex justify-center mt-4">
        <Button type="primary" onClick={handleCompareClick}>
          So sánh
        </Button>
      </div>
    </div>
  );
};

export default CompareModal;
