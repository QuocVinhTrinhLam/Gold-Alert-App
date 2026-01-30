import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { api, type InsertAlert, type Alert } from "@shared/routes";
import { apiRequest } from "@/lib/queryClient";
import { useToast } from "@/hooks/use-toast";

export function useAlerts() {
  return useQuery<Alert[]>({
    queryKey: [api.alerts.list.path],
    queryFn: async () => {
      const res = await fetch(api.alerts.list.path);
      if (!res.ok) throw new Error("Failed to fetch alerts");
      return await res.json();
    },
  });
}

export function useCreateAlert() {
  const queryClient = useQueryClient();
  const { toast } = useToast();

  return useMutation({
    mutationFn: async (data: InsertAlert) => {
      // API expects number for targetPrice, form sends string usually, handled via Zod coercion in schema if set, 
      // but here we ensure it's correct from frontend too.
      // Schema defines targetPrice as integer.
      const res = await apiRequest("POST", api.alerts.create.path, data);
      return await res.json();
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [api.alerts.list.path] });
      toast({
        title: "Thành công",
        description: "Đã tạo cảnh báo giá mới.",
        className: "border-l-4 border-l-yellow-500",
      });
    },
    onError: (error: Error) => {
      toast({
        title: "Lỗi",
        description: error.message || "Không thể tạo cảnh báo.",
        variant: "destructive",
      });
    },
  });
}

export function useDeleteAlert() {
  const queryClient = useQueryClient();
  const { toast } = useToast();

  return useMutation({
    mutationFn: async (id: number) => {
      const path = api.alerts.delete.path.replace(":id", id.toString());
      await apiRequest("DELETE", path);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [api.alerts.list.path] });
      toast({
        title: "Đã xóa",
        description: "Cảnh báo đã được xóa khỏi danh sách.",
      });
    },
    onError: () => {
      toast({
        title: "Lỗi",
        description: "Không thể xóa cảnh báo.",
        variant: "destructive",
      });
    },
  });
}
