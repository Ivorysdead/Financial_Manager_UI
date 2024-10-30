export async function getTotalLoanAmount() {
    const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;

    return fetch(`${apiBaseUrl}/GetTotalLoanAmount`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
    })
        .then(async (response) => {
            if (!response.ok) {
                throw new Error(`Error fetching total amount: ${response.statusText}`);
            }
            const data = await response.json();
            return data.TotalLoanAmount;
        })
        .catch((error) => {
            console.error('Error fetching total amount:', error);
            return 0;
        });
}